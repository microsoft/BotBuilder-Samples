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
        "test": "nyc mocha tests/**/*.test.js"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "@microsoft/recognizers-text-data-types-timex-expression": "1.1.4",
        "botbuilder": "~4.5.1",
        "botbuilder-ai": "~4.5.1",
        "botbuilder-dialogs": "~4.5.1",
        "botbuilder-testing": "~4.5.1",
        "dotenv": "~8.0.0",
        "restify": "~8.3.3"
    },
    "devDependencies": {
        "eslint": "^6.0.1",
        "eslint-config-standard": "^13.0.1",
        "eslint-plugin-import": "^2.18.2",
        "eslint-plugin-node": "^9.1.0",
        "eslint-plugin-promise": "^4.2.1",
        "eslint-plugin-standard": "^4.0.0",
        "nodemon": "~1.19.1",
        "mocha": "^6.2.0",
        "nyc": "^14.1.1"
    }
}
