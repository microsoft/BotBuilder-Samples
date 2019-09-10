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
        "test": "echo \"Error: no test specified\" && exit 1",
        "provision": "node ./scripts/provision.js"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "botbuilder": "~4.5.1",
        "dotenv": "~8.0.0",
        "restify": "~8.3.3"
    },
    "devDependencies": {
        "@azure/arm-resources": "~1.1.0",
        "@azure/arm-subscriptions": "^2.0.0",
        "@azure/graph": "~5.0.0",
        "@azure/ms-rest-js": "^2.0.4",
        "@azure/ms-rest-nodeauth": "~3.0.1",
        "eslint": "^6.0.1",
        "eslint-config-standard": "^13.0.1",
        "eslint-plugin-import": "^2.18.2",
        "eslint-plugin-node": "^9.1.0",
        "eslint-plugin-promise": "^4.2.1",
        "eslint-plugin-standard": "^4.0.0",
        "prompts": "~2.2.1",
        "nodemon": "~1.19.1"
    }
}
