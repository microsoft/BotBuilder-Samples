# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import uuid
from typing import Tuple

from store.store import Store


class MemoryStore(Store):
    """
    Implementation of the IStore abstraction intended for testing.
    """

    def __init__(self):
        # dict of Tuples
        self.store = {}

    async def load(self, key: str) -> ():
        return self.store[key] if key in self.store else (None, None)

    async def save(self, key: str, content, e_tag: str) -> bool:
        if e_tag:
            value: Tuple = self.store[key]
            if value and value[1] != e_tag:
                return False

        self.store[key] = (content, str(uuid.uuid4()))
        return True
