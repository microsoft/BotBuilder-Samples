{
    "name": "<%= botname %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Generated using Microsoft Bot Builder Yeoman generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "start": "node ./index.js",
        "watch": "nodemon ./index.js",
        "lint": "./node_modules/.bin/eslint .",
        "test": "echo \"Error: no test specified\" && exit 1"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "botbuilder": "^4.2.0",
        "botbuilder-ai": "^4.2.0",
        "botbuilder-core": "^4.2.0",
        "botbuilder-dialogs": "^4.2.0",
        "botframework-config": "^4.2.0",
        "botframework-connector": "^4.2.0",
        "botframework-schema": "^4.2.0",
        "dotenv": "^6.1.0",
        "restify": "^7.2.3"
    },
    "devDependencies": {
        "eslint": "^5.8.0",
        "eslint-config-standard": "^12.0.0",
        "eslint-plugin-import": "^2.14.0",
        "eslint-plugin-node": "^8.0.0",
        "eslint-plugin-promise": "^4.0.1",
        "eslint-plugin-standard": "^4.0.0",
        "nodemon": "^1.18.7"
    }
}
