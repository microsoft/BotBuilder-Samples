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
        "watch": "nodemon --watch ./src -e ts --exec \"npm run start\"",
        "provision": "node ./scripts/provision.js"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "botbuilder": "~4.5.1",
        "dotenv": "~8.0.0",
        "replace": "~1.1.0",
        "restify": "~8.3.3"
    },
    "devDependencies": {
        "@azure/arm-resources": "~1.1.0",
        "@azure/arm-subscriptions": "^2.0.0",
        "@azure/graph": "~5.0.0",
        "@azure/ms-rest-js": "^2.0.4",
        "@azure/ms-rest-nodeauth": "~3.0.1",
        "@types/dotenv": "6.1.1",
        "@types/prompts": "~2.0.1",
        "@types/restify": "7.2.12",
        "nodemon": "~1.19.1",
        "tslint": "~5.18.0",
        "prompts": "~2.2.1",
        "typescript": "~3.5.3"
    }
}
