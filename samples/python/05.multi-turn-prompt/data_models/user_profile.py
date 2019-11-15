# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
  This is our application state. Just a regular serializable Python class.
"""


class UserProfile:
    def __init__(self, name: str = None, transport: str = None, age: int = 0):
        self.name = name
        self.transport = transport
        self.age = age
