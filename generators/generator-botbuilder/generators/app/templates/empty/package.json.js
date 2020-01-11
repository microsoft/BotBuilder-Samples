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
        "botbuilder": "~4.7.0",
        "restify": "~8.4.0"
    },
    "devDependencies": {
        "eslint": "^6.6.0",
        "eslint-config-standard": "^14.1.0",
        "eslint-plugin-import": "^2.18.2",
        "eslint-plugin-node": "^10.0.0",
        "eslint-plugin-promise": "^4.2.1",
        "eslint-plugin-standard": "^4.0.1",
        "nodemon": "~1.19.4"
    }
}
