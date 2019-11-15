# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Docker initialization.
This is called from the Dockerfile when creating the model runtime service API
container.
"""
import os
from pathlib import Path
from model_corebot101.language_helper import LanguageHelper

# Initialize the models
LH = LanguageHelper()
HOME_DIR = str(Path.home())
BERT_MODEL_DIR_DEFAULT = os.path.abspath(os.path.join(HOME_DIR, "models/bert"))
BIDAF_MODEL_DIR_DEFAULT = os.path.abspath(os.path.join(HOME_DIR, "models/bidaf"))

LH.initialize_models(
    bert_model_dir=BERT_MODEL_DIR_DEFAULT, bidaf_model_dir=BIDAF_MODEL_DIR_DEFAULT
)
