# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.


class ExampleData(object):
    def __init__(
        self,
        question: str = None,
        is_multi_select: bool = False,
        option1: str = None,
        option2: str = None,
        option3: str = None,
    ):
        self.question = question
        self.is_multi_select = is_multi_select
        self.option1 = option1
        self.option2 = option2
        self.option3 = option3
