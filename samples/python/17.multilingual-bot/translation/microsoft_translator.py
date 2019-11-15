# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import uuid
import requests


class MicrosoftTranslator:
    def __init__(self, subscription_key: str, subscription_region: str):
        self.subscription_key = subscription_key
        self.subscription_region = subscription_region

    # Don't forget to replace with your Cog Services location!
    # Our Flask route will supply two arguments: text_input and language_output.
    # When the translate text button is pressed in our Flask app, the Ajax request
    # will grab these values from our web app, and use them in the request.
    # See main.js for Ajax calls.
    async def translate(self, text_input, language_output):
        base_url = "https://api.cognitive.microsofttranslator.com"
        path = "/translate?api-version=3.0"
        params = "&to=" + language_output
        constructed_url = base_url + path + params

        headers = {
            "Ocp-Apim-Subscription-Key": self.subscription_key,
            "Ocp-Apim-Subscription-Region": self.subscription_region,
            "Content-type": "application/json",
            "X-ClientTraceId": str(uuid.uuid4()),
        }

        # You can pass more than one object in body.
        body = [{"text": text_input}]
        response = requests.post(constructed_url, headers=headers, json=body)
        json_response = response.json()

        # for this sample, return the first translation
        return json_response[0]["translations"][0]["text"]
