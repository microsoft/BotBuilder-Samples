# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Arguments for the model. """

import os
import sys
from pathlib import Path

# pylint:disable=line-too-long
class Args:
    """Arguments for the model."""

    training_data_dir: str = None
    bert_model: str = None
    task_name: str = None
    model_dir: str = None
    cleanup_output_dir: bool = False
    cache_dir: str = ""
    max_seq_length: int = 128
    do_train: bool = None
    do_eval: bool = None
    do_lower_case: bool = None
    train_batch_size: int = 4
    eval_batch_size: int = 8
    learning_rate: float = 5e-5
    num_train_epochs: float = 3.0
    warmup_proportion: float = 0.1
    no_cuda: bool = None
    local_rank: int = -1
    seed: int = 42
    gradient_accumulation_steps: int = 1
    fp16: bool = None
    loss_scale: float = 0

    @classmethod
    def for_flight_booking(
        cls,
        training_data_dir: str = os.path.abspath(
            os.path.join(os.path.dirname(os.path.abspath(__file__)), "../training_data")
        ),
        task_name: str = "flight_booking",
    ):
        """Return the flight booking args."""
        args = cls()

        args.training_data_dir = training_data_dir
        args.task_name = task_name
        home_dir = str(Path.home())
        args.model_dir = os.path.abspath(os.path.join(home_dir, "models/bert"))
        args.bert_model = "bert-base-uncased"
        args.do_lower_case = True

        print(
            f"Bert Model training_data_dir is set to {args.training_data_dir}",
            file=sys.stderr,
        )
        print(f"Bert Model model_dir is set to {args.model_dir}", file=sys.stderr)
        return args
