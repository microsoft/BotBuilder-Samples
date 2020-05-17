# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""Booking detail."""


class BookingDetails:
    """Booking detail implementation"""

    def __init__(
        self, destination: str = None, origin: str = None, travel_date: str = None
    ):
        self.destination = destination
        self.origin = origin
        self.travel_date = travel_date
