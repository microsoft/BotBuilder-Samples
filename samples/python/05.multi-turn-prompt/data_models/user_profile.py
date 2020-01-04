# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.schema import Attachment


class UserProfile:
    """
      This is our application state. Just a regular serializable Python class.
    """

    def __init__(self, name: str = None, transport: str = None, age: int = 0, picture: Attachment = None):
        self.name = name
        self.transport = transport
        self.age = age
        self.picture = picture
