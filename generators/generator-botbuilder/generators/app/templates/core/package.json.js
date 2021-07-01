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
        "lint": "eslint .",
        "test": "echo \"Error: no test specified\" && exit 1"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "@microsoft/recognizers-text-data-types-timex-expression": "1.1.4",
        "botbuilder": "~4.14.0",
        "botbuilder-ai": "~4.14.0",
        "botbuilder-dialogs": "~4.14.0",
        "dotenv": "~8.2.0",
        "restify": "~8.5.1"
    },
    "devDependencies": {
        "eslint": "^7.0.0",
        "eslint-config-standard": "^14.1.1",
        "eslint-plugin-import": "^2.20.2",
        "eslint-plugin-node": "^11.1.0",
        "eslint-plugin-promise": "^4.2.1",
        "eslint-plugin-standard": "^4.0.1",
        "mocha": "^7.1.2",
        "nodemon": "^2.0.4",
        "nyc": "^15.0.1"
    }
}
