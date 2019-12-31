# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import StatePropertyAccessor, TurnContext


class RefAccessor(StatePropertyAccessor):
    """
    This is an accessor for any object. By definition objects (as opposed to values)
    are returned by reference in the GetAsync call on the accessor. As such the SetAsync
    call is never used. The actual act of saving any state to an external store therefore
    cannot be encapsulated in the Accessor implementation itself. And so to facilitate this
    the state itself is available as a public property on this class. The reason its here is
    because the caller of the constructor could pass in null for the state, in which case
    the factory provided on the GetAsync call will be used.
    """

    def __init__(self, value):
        self.value = value
        self.name = type(value).__name__

    async def get(
        self, turn_context: TurnContext, default_value_or_factory=None
    ) -> object:
        if not self.value:
            if not default_value_or_factory:
                raise Exception("key not found")

            self.value = default_value_or_factory()

        return self.value

    async def delete(self, turn_context: TurnContext) -> None:
        pass

    async def set(self, turn_context: TurnContext, value) -> None:
        pass
