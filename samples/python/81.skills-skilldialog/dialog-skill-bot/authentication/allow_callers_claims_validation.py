from typing import Awaitable, Callable, Dict, List

from botframework.connector.auth import JwtTokenValidation, SkillValidation

from config import DefaultConfig


class AllowedCallersClaimsValidator:
    """
    Sample claims validator that loads an allowed list from configuration if present
    and checks that requests are coming from allowed parent bots.
    """

    config_key = "ALLOWED_CALLERS"

    def __init__(self, config: DefaultConfig):
        if not config:
            raise TypeError(
                "AllowedCallersClaimsValidator: config object cannot be None."
            )

        # AllowedCallers is the setting in the appsettings.json file
        # that consists of the list of parent bot IDs that are allowed to access the skill.
        # To add a new parent bot, simply edit the AllowedCallers and add
        # the parent bot's Microsoft app ID to the list.
        # In this sample, we allow all callers if AllowedCallers contains an "*".
        caller_list = getattr(config, self.config_key)
        if caller_list is None:
            raise TypeError(f'"{self.config_key}" not found in configuration.')
        self._allowed_callers = frozenset(caller_list)

    @property
    def claims_validator(self) -> Callable[[List[Dict]], Awaitable]:
        async def allow_callers_claims_validator(claims: Dict[str, object]):
            # If _allowed_callers contains an "*", we allow all callers.
            if "*" not in self._allowed_callers and SkillValidation.is_skill_claim(
                claims
            ):
                # Check that the appId claim in the skill request is in the list of callers configured for this bot.
                app_id = JwtTokenValidation.get_app_id_from_claims(claims)
                if app_id not in self._allowed_callers:
                    raise PermissionError(
                        f'Received a request from a bot with an app ID of "{app_id}".'
                        f" To enable requests from this caller, add the app ID to your configuration file."
                    )

            return

        return allow_callers_claims_validator
