# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import logging
import os
import random
import shutil
import numpy as np
import torch
from .args import Args

from model_corebot101.bert.common.bert_util import BertUtil
from model_corebot101.bert.train.flight_booking_processor import FlightBookingProcessor
from pytorch_pretrained_bert.file_utils import (
    CONFIG_NAME,
    PYTORCH_PRETRAINED_BERT_CACHE,
    WEIGHTS_NAME,
)
from pytorch_pretrained_bert.modeling import (
    BertForSequenceClassification,
    BertPreTrainedModel,
)
from pytorch_pretrained_bert.optimization import BertAdam
from pytorch_pretrained_bert.tokenization import BertTokenizer
from torch.nn import CrossEntropyLoss
from torch.utils.data import DataLoader, RandomSampler, SequentialSampler, TensorDataset
from torch.utils.data.distributed import DistributedSampler

from tqdm import tqdm, trange


class BertTrainEval:
    logger = logging.getLogger(__name__)

    def __init__(self, args: Args):
        self.processor = FlightBookingProcessor()
        self.output_mode = "classification"
        self.args = args
        self._prepare()
        self.model = self._prepare_model()

    @classmethod
    def train_eval(cls, cleanup_output_dir: bool = False) -> None:
        # uncomment the following line for debugging.
        # import pdb; pdb.set_trace()
        args = Args.for_flight_booking()
        args.do_train = True
        args.do_eval = True
        args.cleanup_output_dir = cleanup_output_dir
        bert = cls(args)
        bert.train()
        bert.eval()

    def train(self) -> None:
        # Prepare optimizer
        param_optimizer = list(self.model.named_parameters())
        no_decay = ["bias", "LayerNorm.bias", "LayerNorm.weight"]
        optimizer_grouped_parameters = [
            {
                "params": [
                    p for n, p in param_optimizer if not any(nd in n for nd in no_decay)
                ],
                "weight_decay": 0.01,
            },
            {
                "params": [
                    p for n, p in param_optimizer if any(nd in n for nd in no_decay)
                ],
                "weight_decay": 0.0,
            },
        ]
        optimizer = BertAdam(
            optimizer_grouped_parameters,
            lr=self.args.learning_rate,
            warmup=self.args.warmup_proportion,
            t_total=self.num_train_optimization_steps,
        )

        global_step: int = 0
        nb_tr_steps = 0
        tr_loss: float = 0
        train_features = BertUtil.convert_examples_to_features(
            self.train_examples,
            self.label_list,
            self.args.max_seq_length,
            self.tokenizer,
            self.output_mode,
        )
        self.logger.info("***** Running training *****")
        self.logger.info("  Num examples = %d", len(self.train_examples))
        self.logger.info("  Batch size = %d", self.args.train_batch_size)
        self.logger.info("  Num steps = %d", self.num_train_optimization_steps)
        all_input_ids = torch.tensor(
            [f.input_ids for f in train_features], dtype=torch.long
        )
        all_input_mask = torch.tensor(
            [f.input_mask for f in train_features], dtype=torch.long
        )
        all_segment_ids = torch.tensor(
            [f.segment_ids for f in train_features], dtype=torch.long
        )

        if self.output_mode == "classification":
            all_label_ids = torch.tensor(
                [f.label_id for f in train_features], dtype=torch.long
            )

        train_data = TensorDataset(
            all_input_ids, all_input_mask, all_segment_ids, all_label_ids
        )
        if self.args.local_rank == -1:
            train_sampler = RandomSampler(train_data)
        else:
            train_sampler = DistributedSampler(train_data)
        train_dataloader = DataLoader(
            train_data, sampler=train_sampler, batch_size=self.args.train_batch_size
        )

        self.model.train()
        for _ in trange(int(self.args.num_train_epochs), desc="Epoch"):
            tr_loss = 0
            nb_tr_examples, nb_tr_steps = 0, 0
            for step, batch in enumerate(tqdm(train_dataloader, desc="Iteration")):
                batch = tuple(t.to(self.device) for t in batch)
                input_ids, input_mask, segment_ids, label_ids = batch

                # define a new function to compute loss values for both output_modes
                logits = self.model(input_ids, segment_ids, input_mask, labels=None)

                if self.output_mode == "classification":
                    loss_fct = CrossEntropyLoss()
                    loss = loss_fct(
                        logits.view(-1, self.num_labels), label_ids.view(-1)
                    )

                if self.args.gradient_accumulation_steps > 1:
                    loss = loss / self.args.gradient_accumulation_steps

                loss.backward()

                tr_loss += loss.item()
                nb_tr_examples += input_ids.size(0)
                nb_tr_steps += 1
                if (step + 1) % self.args.gradient_accumulation_steps == 0:
                    optimizer.step()
                    optimizer.zero_grad()
                    global_step += 1

        if self.args.local_rank == -1 or torch.distributed.get_rank() == 0:
            # Save a trained model, configuration and tokenizer
            model_to_save = (
                self.model.module if hasattr(self.model, "module") else self.model
            )  # Only save the model it-self

            # If we save using the predefined names, we can load using `from_pretrained`
            output_model_file = os.path.join(self.args.model_dir, WEIGHTS_NAME)
            output_config_file = os.path.join(self.args.model_dir, CONFIG_NAME)

            torch.save(model_to_save.state_dict(), output_model_file)
            model_to_save.config.to_json_file(output_config_file)
            self.tokenizer.save_vocabulary(self.args.model_dir)

            # Load a trained model and vocabulary that you have fine-tuned
            self.model = BertForSequenceClassification.from_pretrained(
                self.args.model_dir, num_labels=self.num_labels
            )
            self.tokenizer = BertTokenizer.from_pretrained(
                self.args.model_dir, do_lower_case=self.args.do_lower_case
            )
        else:
            self.model = BertForSequenceClassification.from_pretrained(
                self.args.bert_model, num_labels=self.num_labels
            )
        self.model.to(self.device)

        self.tr_loss, self.global_step = tr_loss, global_step

        self.logger.info("DONE TRAINING."),

    def eval(self) -> None:
        if not (self.args.local_rank == -1 or torch.distributed.get_rank() == 0):
            return

        eval_examples = self.processor.get_dev_examples(self.args.training_data_dir)
        eval_features = BertUtil.convert_examples_to_features(
            eval_examples,
            self.label_list,
            self.args.max_seq_length,
            self.tokenizer,
            self.output_mode,
        )
        self.logger.info("***** Running evaluation *****")
        self.logger.info("  Num examples = %d", len(eval_examples))
        self.logger.info("  Batch size = %d", self.args.eval_batch_size)
        all_input_ids = torch.tensor(
            [f.input_ids for f in eval_features], dtype=torch.long
        )
        all_input_mask = torch.tensor(
            [f.input_mask for f in eval_features], dtype=torch.long
        )
        all_segment_ids = torch.tensor(
            [f.segment_ids for f in eval_features], dtype=torch.long
        )

        if self.output_mode == "classification":
            all_label_ids = torch.tensor(
                [f.label_id for f in eval_features], dtype=torch.long
            )

        eval_data = TensorDataset(
            all_input_ids, all_input_mask, all_segment_ids, all_label_ids
        )
        # Run prediction for full data
        eval_sampler = SequentialSampler(eval_data)
        eval_dataloader = DataLoader(
            eval_data, sampler=eval_sampler, batch_size=self.args.eval_batch_size
        )

        self.model.eval()
        eval_loss = 0
        nb_eval_steps = 0
        preds = []

        for input_ids, input_mask, segment_ids, label_ids in tqdm(
            eval_dataloader, desc="Evaluating"
        ):
            input_ids = input_ids.to(self.device)
            input_mask = input_mask.to(self.device)
            segment_ids = segment_ids.to(self.device)
            label_ids = label_ids.to(self.device)

            with torch.no_grad():
                logits = self.model(input_ids, segment_ids, input_mask, labels=None)

            # create eval loss and other metric required by the task
            if self.output_mode == "classification":
                loss_fct = CrossEntropyLoss()
                tmp_eval_loss = loss_fct(
                    logits.view(-1, self.num_labels), label_ids.view(-1)
                )

            eval_loss += tmp_eval_loss.mean().item()
            nb_eval_steps += 1
            if len(preds) == 0:
                preds.append(logits.detach().cpu().numpy())
            else:
                preds[0] = np.append(preds[0], logits.detach().cpu().numpy(), axis=0)

        eval_loss = eval_loss / nb_eval_steps
        preds = preds[0]
        if self.output_mode == "classification":
            preds = np.argmax(preds, axis=1)
        result = BertUtil.compute_metrics(self.task_name, preds, all_label_ids.numpy())
        loss = self.tr_loss / self.global_step if self.args.do_train else None

        result["eval_loss"] = eval_loss
        result["global_step"] = self.global_step
        result["loss"] = loss

        output_eval_file = os.path.join(self.args.model_dir, "eval_results.txt")
        with open(output_eval_file, "w") as writer:
            self.logger.info("***** Eval results *****")
            for key in sorted(result.keys()):
                self.logger.info("  %s = %s", key, str(result[key]))
                writer.write("%s = %s\n" % (key, str(result[key])))

        self.logger.info("DONE EVALUATING.")

    def _prepare(self, cleanup_output_dir: bool = False) -> None:
        if self.args.local_rank == -1 or self.args.no_cuda:
            self.device = torch.device(
                "cuda" if torch.cuda.is_available() and not self.args.no_cuda else "cpu"
            )
            self.n_gpu = torch.cuda.device_count()
        else:
            torch.cuda.set_device(self.args.local_rank)
            self.device = torch.device("cuda", self.args.local_rank)
            self.n_gpu = 1
            # Initializes the distributed backend which will take care of sychronizing nodes/GPUs
            torch.distributed.init_process_group(backend="nccl")

        logging.basicConfig(
            format="%(asctime)s - %(levelname)s - %(name)s -   %(message)s",
            datefmt="%m/%d/%Y %H:%M:%S",
            level=logging.INFO if self.args.local_rank in [-1, 0] else logging.WARN,
        )

        self.logger.info(
            "device: {} n_gpu: {}, distributed training: {}, 16-bits training: {}".format(
                self.device,
                self.n_gpu,
                bool(self.args.local_rank != -1),
                self.args.fp16,
            )
        )

        if self.args.gradient_accumulation_steps < 1:
            raise ValueError(
                "Invalid gradient_accumulation_steps parameter: {}, should be >= 1".format(
                    self.args.gradient_accumulation_steps
                )
            )

        self.args.train_batch_size = (
            self.args.train_batch_size // self.args.gradient_accumulation_steps
        )

        random.seed(self.args.seed)
        np.random.seed(self.args.seed)
        torch.manual_seed(self.args.seed)
        if self.n_gpu > 0:
            torch.cuda.manual_seed_all(self.args.seed)

        if not self.args.do_train and not self.args.do_eval:
            raise ValueError("At least one of `do_train` or `do_eval` must be True.")

        if self.args.cleanup_output_dir:
            if os.path.exists(self.args.model_dir):
                shutil.rmtree(self.args.model_dir)

        if (
            os.path.exists(self.args.model_dir)
            and os.listdir(self.args.model_dir)
            and self.args.do_train
        ):
            raise ValueError(
                "Output directory ({}) already exists and is not empty.".format(
                    self.args.model_dir
                )
            )
        if not os.path.exists(self.args.model_dir):
            os.makedirs(self.args.model_dir)

        self.task_name = self.args.task_name.lower()

        self.label_list = self.processor.get_labels()
        self.num_labels = len(self.label_list)

        self.tokenizer = BertTokenizer.from_pretrained(
            self.args.bert_model, do_lower_case=self.args.do_lower_case
        )

        self.train_examples = None
        self.num_train_optimization_steps = None
        if self.args.do_train:
            self.train_examples = self.processor.get_train_examples(
                self.args.training_data_dir
            )
            self.num_train_optimization_steps = (
                int(
                    len(self.train_examples)
                    / self.args.train_batch_size
                    / self.args.gradient_accumulation_steps
                )
                * self.args.num_train_epochs
            )
            if self.args.local_rank != -1:
                self.num_train_optimization_steps = (
                    self.num_train_optimization_steps
                    // torch.distributed.get_world_size()
                )

    def _prepare_model(self) -> BertPreTrainedModel:
        if self.args.cache_dir:
            cache_dir = self.args.cache_dir
        else:
            cache_dir = os.path.join(
                str(PYTORCH_PRETRAINED_BERT_CACHE),
                f"distributed_{self.args.local_rank}",
            )
        model = BertForSequenceClassification.from_pretrained(
            self.args.bert_model, cache_dir=cache_dir, num_labels=self.num_labels
        )
        model.to(self.device)
        return model
