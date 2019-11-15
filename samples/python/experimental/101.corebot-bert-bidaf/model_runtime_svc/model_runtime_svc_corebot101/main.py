# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Model Runtime.
Entry point for the model runtime.
"""
import os
import signal
import logging
from logging.handlers import RotatingFileHandler
import tornado
from tornado.options import define, options
from pathlib import Path
from model_corebot101.language_helper import LanguageHelper
from handlers.model_handler import ModelHandler

HOME_DIR = str(Path.home())

# Define Tornado options
define("port", default=8880, help="HTTP port for model runtime to listen on", type=int)
define(
    "bidaf_model_dir",
    default=os.path.abspath(os.path.join(HOME_DIR, "models/bidaf")),
    help="bidaf model directory",
)
define(
    "bert_model_dir",
    default=os.path.abspath(os.path.join(HOME_DIR, "models/bert")),
    help="bert model directory",
)


def setup_logging():
    """Set up logging."""
    logging.info("Setting up logging infrastructure")

    # Create the rotating log handler
    if not os.path.exists("logs"):
        os.mkdir("logs")
    handler = RotatingFileHandler(
        os.path.join("./logs", "model-runtime.log"),
        maxBytes=5 * 1024 ** 2,  # 5 MB chunks,
        backupCount=5,  # limit to 25 MB logs max
    )

    # Set the formatter
    handler.setFormatter(
        logging.Formatter("%(asctime)s %(name)-12s %(levelname)-8s %(message)s")
    )

    # Setup the root logging with the necessary handlers
    log = logging.getLogger()
    log.addHandler(handler)

    # Set to info for normal processing
    log.setLevel(logging.INFO)


# pylint:disable=unused-argument
def signal_handler(sig_num, frame):
    """Stop activity on signal."""
    tornado.ioloop.IOLoop.instance().stop()


def run():
    """Main entry point for model runtime api."""

    # Register signal handlers.
    logging.info("Preparing signal handlers..")
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)

    # Set up model cache.
    # If containerizing, suggest initializing the directories (and associated
    # file downloads) be performed during container build time.
    logging.info("Initializing model directories:")
    logging.info("    bert  : %s", options.bert_model_dir)
    logging.info("    bidaf : %s", options.bidaf_model_dir)

    language_helper = LanguageHelper()
    if (
        language_helper.initialize_models(
            options.bert_model_dir, options.bidaf_model_dir
        )
        is False
    ):
        logging.error("Could not initilize model directories.  Exiting..")
        return

    # Build the configuration
    logging.info("Building config..")
    ref_obj = {"language_helper": language_helper}
    app_config = ModelHandler.build_config(ref_obj)

    logging.info("Starting Tornado model runtime service..")
    application = tornado.web.Application(app_config)
    application.listen(options.port)

    # Protect the loop with a try/catch
    try:
        # Start the app and wait for a close
        tornado.ioloop.IOLoop.instance().start()
    finally:
        # handle error with shutting down loop
        tornado.ioloop.IOLoop.instance().stop()


if __name__ == "__main__":
    setup_logging()
    run()
