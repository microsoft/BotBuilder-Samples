{
    "name": "<%= botname %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Generated using Microsoft Bot Builder Yeoman generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "build": "tsc --build",
        "lint": "tslint -c tslint.json 'src/**/*.ts'",
        "postinstall": "npm run build && node ./deploymentScripts/webConfigPrep.js",
        "start": "tsc --build && node ./lib/index.js",
        "test": "echo \"Error: no test specified\" && exit 1",
        "watch": "nodemon --watch ./src -e ts --exec \"npm run start\""
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "@microsoft/recognizers-text-data-types-timex-expression": "1.1.4",
        "botbuilder": "~4.5.0",
        "botbuilder-ai": "~4.5.0",
        "botbuilder-dialogs": "~4.5.0",
        "dotenv": "^7.0.0",
        "replace": "~1.1.0",
        "restify": "~8.2.0"
    },
    "devDependencies": {
        "@types/dotenv": "6.1.0",
        "@types/restify": "7.2.9",
        "nodemon": "~1.18.10",
        "tslint": "~5.14.0",
        "typescript": "~3.3.3"
    }
}
