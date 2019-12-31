# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json

from azure.storage.blob import BlockBlobService, PublicAccess
from jsonpickle import encode
from jsonpickle.unpickler import Unpickler

from store.store import Store


class BlobStore(Store):
    """
    An implementation of the ETag aware Store interface against Azure Blob Storage.
    """

    def __init__(self, account_name: str, account_key: str, container_name: str):
        self.container_name = container_name
        self.client = BlockBlobService(
            account_name=account_name, account_key=account_key
        )

    async def load(self, key: str) -> ():
        self.client.create_container(self.container_name)
        self.client.set_container_acl(
            self.container_name, public_access=PublicAccess.Container
        )

        if not self.client.exists(container_name=self.container_name, blob_name=key):
            return None, None

        blob = self.client.get_blob_to_text(
            container_name=self.container_name, blob_name=key
        )
        return Unpickler().restore(json.loads(blob.content)), blob.properties.etag

    async def save(self, key: str, content, e_tag: str):
        self.client.create_container(self.container_name)
        self.client.set_container_acl(
            self.container_name, public_access=PublicAccess.Container
        )

        self.client.create_blob_from_text(
            container_name=self.container_name,
            blob_name=key,
            text=encode(content),
            if_match=e_tag,
        )

        return True
