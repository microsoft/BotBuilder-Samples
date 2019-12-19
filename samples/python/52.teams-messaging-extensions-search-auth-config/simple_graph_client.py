# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json
from urllib.parse import urlparse, urljoin, quote

from requests_oauthlib import OAuth2Session

RESOURCE = "https://graph.microsoft.com"
API_VERSION = "beta"


class SimpleGraphClient:
    def __init__(self, token: str):
        self.token = token
        self.client = OAuth2Session(
            token={"access_token": token, "token_type": "Bearer"}
        )

    async def search_mail_inbox(self, search_query: str):
        query = quote(search_query)  # cleans up the url

        # Searches the user's mail Inbox using the Microsoft Graph API
        # https://graph.microsoft.com/beta/search/query
        search_filter = {
            "requests": [
                {
                    "entityTypes": ["microsoft.graph.message"],
                    "query": {"query_string": {"query": query}},
                    "from": 0,
                    "size": 20,
                }
            ]
        }
        response = self.client.post(
            self.api_endpoint(f"search/query"),
            headers={"Content-Type": "application/json"},
            json=search_filter,
        )
        response_json = json.loads(response.text)
        total_results = response_json["value"][0]["hitsContainers"][0]["total"]
        if int(total_results) > 0:
            return response_json["value"][0]["hitsContainers"][0]["hits"]

        return {}

    def api_endpoint(self, url):
        """Convert a relative path such as /me/photo/$value to a full URI based
        on the current RESOURCE and API_VERSION settings in config.py.
        """
        if urlparse(url).scheme in ["http", "https"]:
            return url  # url is already complete
        return urljoin(f"{RESOURCE}/{API_VERSION}/", url.lstrip("/"))
