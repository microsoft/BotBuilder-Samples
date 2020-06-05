# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.


class Location:
    def __init__(
        self, latitude: str = None, longitude: str = None, postal_code: str = None,
    ):
        self.latitude = latitude
        self.longitude = longitude
        self.postal_code = postal_code

    def from_json(self, json: dict):
        self.latitude = json.get("latitude", None)
        self.longitude = json.get("longitude", None)
        self.postal_code = json.get("postal_code", None)
