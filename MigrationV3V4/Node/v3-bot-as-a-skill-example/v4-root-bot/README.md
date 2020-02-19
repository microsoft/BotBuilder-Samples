# V4 root bot - Using a v3 bot as a skill

## Overview
This sample demonstrates how to consume a v3 bot that's configured as a skill from a v4 bot.

## Setup

1. Create a '.env' file in the root folder and copy / paste the contents of '.env.example' into this new file.
2. Fill in the '.env' values based on your bot's specific configuration

```
MicrosoftAppId={V4_BOT_APP_ID}
MicrosoftAppPassword={V4_BOT_PASSWORD}
SkillHostEndpoint={V3_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/skills/}

SkillId={V3_SKILL_BOT_NAME ex: 'v3-skill-bot'}
SkillAppId={V3_BOT_APP_ID}
SkillEndpoint={V3_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/messages}
```

3. See README.md in 'v3-skill-bot' folder and complete v3 skill setup

## Test run

1. Launch v4 bot and v3 bot ('npm run start' from their respective root folders)

2. Sent test message to v4 bot in order to invoke v3 bot as skill: 'skill'. The v3 bot should echo the message back to v4 and display it in the console (or emulator, etc.)
