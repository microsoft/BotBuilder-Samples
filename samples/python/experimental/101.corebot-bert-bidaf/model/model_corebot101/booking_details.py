# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Booking Details.
The lu model will detect the properties of a flight booking.
"""


class BookingDetails:
    """Booking properties from lu model."""

    def __init__(
        self, destination: str = None, origin: str = None, travel_date: str = None
    ):
        self.destination = destination
        self.origin = origin
        self.travel_date = travel_date
