# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import asyncio

from adapter import ConsoleAdapter
from bot import EchoBot

# Create adapter
ADAPTER = ConsoleAdapter()
BOT = EchoBot()

LOOP = asyncio.get_event_loop()

if __name__ == "__main__":
    try:
        # Greet user
        print("Hi... I'm an echobot. Whatever you say I'll echo back.")

        LOOP.run_until_complete(ADAPTER.process_activity(BOT.on_turn))
    except KeyboardInterrupt:
        pass
    finally:
        LOOP.stop()
        LOOP.close()
