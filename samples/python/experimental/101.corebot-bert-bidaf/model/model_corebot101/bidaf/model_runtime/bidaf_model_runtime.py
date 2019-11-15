# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import os
import sys
import requests
import shutil
from typing import Dict, List, Tuple
import nltk
import numpy as np
from nltk import word_tokenize
from onnxruntime import InferenceSession

# pylint:disable=line-too-long
class BidafModelRuntime:
    def __init__(self, targets: List[str], queries: Dict[str, str], model_dir: str):
        self.queries = queries
        self.targets = targets
        bidaf_model = os.path.abspath(os.path.join(model_dir, "bidaf.onnx"))
        print(f"Loading Inference session from {bidaf_model}..", file=sys.stderr)
        self.session = InferenceSession(bidaf_model)
        print(f"Inference session loaded..", file=sys.stderr)
        self.processed_queries = self._process_queries()
        print(f"Processed queries..", file=sys.stderr)

    @staticmethod
    def init_bidaf(bidaf_model_dir: str, download_ntlk_punkt: bool = False) -> bool:
        if os.path.isdir(bidaf_model_dir):
            print("bidaf model directory already present..", file=sys.stderr)
        else:
            print("Creating bidaf model directory..", file=sys.stderr)
            os.makedirs(bidaf_model_dir, exist_ok=True)

        # Download Punkt Sentence Tokenizer
        if download_ntlk_punkt:
            nltk.download("punkt", download_dir=bidaf_model_dir)
            nltk.download("punkt")

        # Download bidaf onnx model
        onnx_model_file = os.path.abspath(os.path.join(bidaf_model_dir, "bidaf.onnx"))

        print(f"Checking file {onnx_model_file}..", file=sys.stderr)
        if os.path.isfile(onnx_model_file):
            print("bidaf.onnx downloaded already!", file=sys.stderr)
        else:
            print("Downloading bidaf.onnx...", file=sys.stderr)
            response = requests.get(
                "https://onnxzoo.blob.core.windows.net/models/opset_9/bidaf/bidaf.onnx",
                stream=True,
            )
            with open(onnx_model_file, "wb") as f:
                response.raw.decode_content = True
                shutil.copyfileobj(response.raw, f)
        return True

    def serve(self, context: str) -> Dict[str, str]:
        result = {}
        cw, cc = BidafModelRuntime._preprocess(context)
        for target in self.targets:
            qw, qc = self.processed_queries[target]
            answer = self.session.run(
                ["start_pos", "end_pos"],
                {
                    "context_word": cw,
                    "context_char": cc,
                    "query_word": qw,
                    "query_char": qc,
                },
            )
            start = answer[0].item()
            end = answer[1].item()
            result_item = cw[start : end + 1]
            result[target] = BidafModelRuntime._convert_result(result_item)

        return result

    def _process_queries(self) -> Dict[str, Tuple[np.ndarray, np.ndarray]]:
        result = {}
        for target in self.targets:
            question = self.queries[target]
            result[target] = BidafModelRuntime._preprocess(question)

        return result

    @staticmethod
    def _convert_result(result_item: np.ndarray) -> str:
        result = []
        for item in result_item:
            result.append(item[0])

        return " ".join(result)

    @staticmethod
    def _preprocess(text: str) -> Tuple[np.ndarray, np.ndarray]:
        tokens = word_tokenize(text)
        # split into lower-case word tokens, in numpy array with shape of (seq, 1)
        words = np.asarray([w.lower() for w in tokens]).reshape(-1, 1)
        # split words into chars, in numpy array with shape of (seq, 1, 1, 16)
        chars = [[c for c in t][:16] for t in tokens]
        chars = [cs + [""] * (16 - len(cs)) for cs in chars]
        chars = np.asarray(chars).reshape(-1, 1, 1, 16)
        return words, chars
