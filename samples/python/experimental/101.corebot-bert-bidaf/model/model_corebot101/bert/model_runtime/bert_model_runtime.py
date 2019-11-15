# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Bert model runtime."""

import os
import sys
from typing import List
import numpy as np
import torch
from torch.utils.data import DataLoader, SequentialSampler, TensorDataset
from pytorch_pretrained_bert import BertForSequenceClassification, BertTokenizer
from model_corebot101.bert.common.bert_util import BertUtil
from model_corebot101.bert.common.input_example import InputExample


class BertModelRuntime:
    """Model runtime for the Bert model."""

    def __init__(
        self,
        model_dir: str,
        label_list: List[str],
        max_seq_length: int = 128,
        output_mode: str = "classification",
        no_cuda: bool = False,
        do_lower_case: bool = True,
    ):
        self.model_dir = model_dir
        self.label_list = label_list
        self.num_labels = len(self.label_list)
        self.max_seq_length = max_seq_length
        self.output_mode = output_mode
        self.no_cuda = no_cuda
        self.do_lower_case = do_lower_case
        self._load_model()

    # pylint:disable=unused-argument
    @staticmethod
    def init_bert(bert_model_dir: str) -> bool:
        """ Handle any one-time initlization """
        if os.path.isdir(bert_model_dir):
            print("bert model directory already present..", file=sys.stderr)
        else:
            print("Creating bert model directory..", file=sys.stderr)
            os.makedirs(bert_model_dir, exist_ok=True)
        return True

    def _load_model(self) -> None:
        self.device = torch.device(
            "cuda" if torch.cuda.is_available() and not self.no_cuda else "cpu"
        )
        self.n_gpu = torch.cuda.device_count()

        # Load a trained model and vocabulary that you have fine-tuned
        self.model = BertForSequenceClassification.from_pretrained(
            self.model_dir, num_labels=self.num_labels
        )
        self.tokenizer = BertTokenizer.from_pretrained(
            self.model_dir, do_lower_case=self.do_lower_case
        )
        self.model.to(self.device)

    def serve(self, query: str) -> str:
        example = InputExample(
            guid="", text_a=query, text_b=None, label=self.label_list[0]
        )
        examples = [example]

        eval_features = BertUtil.convert_examples_to_features(
            examples,
            self.label_list,
            self.max_seq_length,
            self.tokenizer,
            self.output_mode,
        )
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
        eval_dataloader = DataLoader(eval_data, sampler=eval_sampler, batch_size=1)

        self.model.eval()
        nb_eval_steps = 0
        preds = []

        for input_ids, input_mask, segment_ids, label_ids in eval_dataloader:
            input_ids = input_ids.to(self.device)
            input_mask = input_mask.to(self.device)
            segment_ids = segment_ids.to(self.device)

            with torch.no_grad():
                logits = self.model(input_ids, segment_ids, input_mask, labels=None)

            nb_eval_steps += 1
            if len(preds) == 0:
                preds.append(logits.detach().cpu().numpy())
            else:
                preds[0] = np.append(preds[0], logits.detach().cpu().numpy(), axis=0)

        preds = preds[0]
        if self.output_mode == "classification":
            preds = np.argmax(preds, axis=1)

        label_id = preds[0]
        pred_label = self.label_list[label_id]
        return pred_label
