#!/bin/bash

#
# CLI Argument processing
#
if [ "$1" = help ]
then
    echo ""
    echo "USAGE:  nocleanup or noclean : Do not delete generated bots after they've been created."
    echo ""
    exit
fi


#
# Empty bot in TypeScript
#
echo Generating my-empty-bot-ts
yo botbuilder -N "my-empty-bot-ts" -D "An empty bot in ts" -L "TypeScript" -T "empty" --noprompt
cd ./my-empty-bot-ts
echo building and linting my-empty-bot-ts
npm run build
npm run lint
cd ..


#
# Empty bot in JavaScript
#
echo Generating my-empty-bot-js
yo botbuilder -N "my-empty-bot-js" -D "An empty bot in js" -L "JavaScript" -T "empty" --noprompt
cd ./my-empty-bot-js
echo linting my-empty-bot-js
npm run lint
cd ..


#
# Echo bot in TypeScript
#
echo Generating my-echo-bot-ts
yo botbuilder -N "my-echo-bot-ts" -D "An echo bot in ts" -L "TypeScript" -T "echo" --noprompt
cd ./my-echo-bot-ts
echo building and linting my-echo-bot-ts
npm run build
npm run lint
cd ..


#
# Echo bot in JavaScript
#
echo Generating my-echo-bot-js
yo botbuilder -N "my-echo-bot-js" -D "An echo bot in js" -L "JavaScript" -T "echo" --noprompt
cd ./my-echo-bot-js
echo linting my-echo-bot-js
npm run lint
cd ..


#
# Core bot in TypeScript
#
echo Generating my-core-bot-ts
yo botbuilder -N "my-core-bot-ts" -D "A core bot in ts" -L "TypeScript" -T "core" --noprompt
cd ./my-core-bot-ts
echo building and linting my-core-bot-ts
npm run build
npm run lint
cd ..


#
# Core bot with tests in TypeScript
#
echo Generating my-core-bot-with-tests-ts
yo botbuilder -N "my-core-bot-with-tests-ts" -D "A core bot with tests in ts" -L "TypeScript" -T "core" --addtests --noprompt
cd ./my-core-bot-with-tests-ts
echo building and linting my-core-bot-with-tests-ts
npm run build
npm run lint
npm test
cd ..


#
# Core bot in JavaScript
#
echo Generating my-core-bot-js
yo botbuilder -N "my-core-bot-js" -D "A core bot in js" -L "JavaScript" -T "core" --noprompt
cd ./my-core-bot-js
echo linting my-core-bot-js
npm run lint
cd ..


#
# Core bot with tests in JavaScript
#
echo Generating my-core-bot-with-tests-js
yo botbuilder -N "my-core-bot-with-tests-js" -D "A core bot with tests in js" -L "JavaScript" -T "core" --addtests --noprompt
cd ./my-core-bot-with-tests-js
echo linting my-core-bot-with-tests-js
npm run lint
npm test
cd ..

if [ "$1" = nocleanup ] || [ "$1" = noclean ]
then
    echo "*****************************************************************************"
    echo "** noclean(up) option used.  You must manually clean up all generated bots **"
    echo "*****************************************************************************"
else
    ## Clean up all the generated projects ##
    echo Cleaning up...

    #
    # Empty bot in TypeScript
    #
    rm -rf ./my-empty-bot-ts


    #
    # Empty bot in JavaScript
    #
    rm -rf ./my-empty-bot-js

    #
    # Echo bot in TypeScript
    #
    rm -rf ./my-echo-bot-ts


    #
    # Echo bot in JavaScript
    #
    rm -rf ./my-echo-bot-js


    #
    # Core bot in TypeScript
    #
    rm -rf ./my-core-bot-ts


    #
    # Core bot with tests in TypeScript
    #
    rm -rf ./my-core-bot-with-tests-ts


    #
    # Core bot in JavaScript
    #
    rm -rf ./my-core-bot-js


    #
    # Core bot with tests in JavaScript
    #
    rm -rf ./my-core-bot-with-tests-js
fi
