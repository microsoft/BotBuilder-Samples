# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botframework.connector.auth import JwtTokenValidation, SkillValidation


class AllowedSkillsClaimsValidator:
    """
    Sample claims validator that loads an allowed list from config if present and checks
    that requests are coming from allowed skills.
    """

    def __init__(self, allowed_callers: set):
        self.allowed_callers = allowed_callers

    # Check AppIds for the configured callers (we will only allow responses from skills we have configured).
    # SkillConfiguration.SKILLS is the list of Skill app Ids that are allowed to access the parent.
    # To add a new skill simply go to the config.py file and add
    # the skill's id, Microsoft AppId and skill_endpoint to the array under SKILLS.
    async def validate_claims(self, claims: dict):
        if SkillValidation.is_skill_claim(claims) and self.allowed_callers:
            # Check that the appId claim in the request is in the list of skills configured for this bot.
            app_id = JwtTokenValidation.get_app_id_from_claims(claims)
            if app_id not in self.allowed_callers:
                raise ValueError(
                    f'Received a request from an application with an appID of "{ app_id }". To enable requests from this skill, add the id to your configuration file.'
                )
