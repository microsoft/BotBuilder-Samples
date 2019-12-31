# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from abc import ABC, abstractmethod


class Store(ABC):
    """
    An ETag aware store definition.
    The interface is defined in terms of JObject to move serialization out of the storage layer
    while still indicating it is JSON, a fact the store may choose to make use of.
    """

    @abstractmethod
    async def load(self, key: str) -> ():
        """
        Loads a value from the Store.
        :param key:
        :return: (object, etag)
        """
        raise NotImplementedError

    @abstractmethod
    async def save(self, key: str, content, e_tag: str) -> bool:
        """
        Saves a values to the Store if the etag matches.
        :param key:
        :param content:
        :param e_tag:
        :return:  True if the content was saved.
        """
        raise NotImplementedError
