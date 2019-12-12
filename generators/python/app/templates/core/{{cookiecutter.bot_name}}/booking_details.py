# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List


class BookingDetails:
    def __init__(
        self,
        destination: str = None,
        origin: str = None,
        travel_date: str = None,
        unsupported_airports: List[str] = None,
    ):
        self.destination = destination
        self.origin = origin
        self.travel_date = travel_date
        self.unsupported_airports = unsupported_airports or []
