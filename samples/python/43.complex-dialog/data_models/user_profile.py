# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List


class UserProfile:
    def __init__(
        self, name: str = None, age: int = 0, companies_to_review: List[str] = None
    ):
        self.name: str = name
        self.age: int = age
        self.companies_to_review: List[str] = companies_to_review
