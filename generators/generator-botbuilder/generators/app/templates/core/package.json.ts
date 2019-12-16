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
        "botbuilder": "~4.6.0",
        "botbuilder-ai": "~4.6.0",
        "botbuilder-dialogs": "~4.6.0",
        "dotenv": "^8.2.0",
        "replace": "~1.1.1",
        "restify": "~8.4.0"
    },
    "devDependencies": {
        "@types/dotenv": "6.1.1",
        "@types/mocha": "^5.2.7",
        "@types/restify": "8.4.1",
        "mocha": "^6.2.2",
        "nodemon": "~1.19.4",
        "nyc": "^14.1.1",
        "ts-node": "^8.4.1",
        "tslint": "~5.20.0",
        "typescript": "~3.6.4"
    }
}
