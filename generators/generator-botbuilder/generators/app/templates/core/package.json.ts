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
        "botbuilder": "~4.13.4",
        "botbuilder-ai": "~4.13.4",
        "botbuilder-dialogs": "~4.13.4",
        "dotenv": "~8.2.0",
        "replace": "~1.2.0",
        "restify": "~8.5.1"
    },
    "devDependencies": {
        "@types/mocha": "^7.0.2",
        "@types/restify": "8.4.2",
        "mocha": "^7.1.2",
        "nodemon": "^2.0.4",
        "nyc": "^15.0.1",
        "ts-node": "^8.10.1",
        "tslint": "^6.1.2",
        "typescript": "^3.9.2"
    }
}
