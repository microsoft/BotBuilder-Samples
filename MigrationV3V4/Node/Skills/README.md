# Use a v3 bot as a skill from a v4 bot

## Overview
This sample demonstrates how to consume V3s bot that are configured as skills from a v4 bot.

## Skill bots setup

1. In the 'v3-skill-bot' folders, create a '.env' file in each v3 skill bot root folder and copy / paste the contents of '.env.example' into this new file.
2. Fill in the '.env' values based on your bot's specific configuration

```
MICROSOFT_APP_ID={V3_BOT_APP_ID}
MICROSOFT_APP_PASSWORD={V3_BOT_PASSWORD}
ROOT_BOT_APP_ID={V4_BOT_APP_ID}
```

## Root bot setup

1. In the 'v4-root-bot' folder, create a '.env' file in each v3 skill bot root folder and copy / paste the contents of '.env.example' into this new file.
2. Fill in the '.env' values based on your bot's specific configuration

```
MicrosoftAppId={V4_BOT_APP_ID}
MicrosoftAppPassword={V4_BOT_PASSWORD}
SkillHostEndpoint={V3_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/skills/}

SkillSimpleId={V3_SIMPLE_SKILL_BOT_NAME ex: 'v3-skill-bot'}
SkillSimpleAppId={V3_SIMPLE_SKILL_BOT_APP_ID}
SkillSimpleEndpoint={V3_SIMPLE_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/messages}

SkillBookingId={V3_BOOKING_SKILL_BOT_NAME ex: 'v3-booking-bot-skill'}
SkillBookingAppId={V3_BOOKING_SKILL_BOT_APP_ID}
SkillBookingEndpoint={V3_BOOKING_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/messages}
```

## Test run

1. Launch v4 bot and v3 bot(s) ('npm run start' from their respective root folders)
2. Sent test message to v4 bot in order to invoke v3 bot as skill. You should be prompted to enter skecific test in order to invoke one of the two skills and enter their respective dialog flows. Enter 'end' at any time to return to the parent bot.