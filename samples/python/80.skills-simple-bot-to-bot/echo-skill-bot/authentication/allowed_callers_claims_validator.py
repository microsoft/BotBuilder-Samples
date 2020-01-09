# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botframework.connector.auth import JwtTokenValidation, SkillValidation


class AllowedCallersClaimsValidator():
    """
    Sample claims validator that loads an allowed list from configuration if present and checks
    that requests are coming from allowed parent bots.
    """

    def __init__(self, allowed_callers: frozenset):
        self.allowed_callers = allowed_callers

    # Load the AppIds for the configured callers (we will only allow responses from parent bots we have configured).
    # DefaultConfig.ALLOWED_CALLERS is the list of parent bot Ids that are allowed to access the skill
    # to add a new parent bot simply go to the config.py file and add
    # the parent bot's Microsoft AppId to the array under AllowedCallers, e.g.:
    # AllowedCallers=["195bd793-4319-4a84-a800-386770c058b2","38c74e7a-3d01-4295-8e66-43dd358920f8"]
    async def validate_claims(self, claims: dict):
        if SkillValidation.is_skill_claim(claims) and self.allowed_callers:
            # Check that the appId claim in the skill request is in the list of skills configured for this bot.
            app_id = JwtTokenValidation.get_app_id_from_claims(claims)
            if app_id not in self.allowed_callers:
                raise ValueError(
                    f'Received a request from an application with an appID of "{ app_id }". To enable requests from this bot, add the id to your configuration file.'
                )