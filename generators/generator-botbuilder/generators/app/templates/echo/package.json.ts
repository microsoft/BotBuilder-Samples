{
    "name": "<%= botName %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Microsoft Bot Builder Yeoman Generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "test": "echo \"Error: no test specified\" && exit 1",
        "build": "tsc",
        "start": "tsc && node ./lib/index.js",
        "watch": "concurrently --kill-others \"tsc -w\" \"nodemon ./lib/index.js\""
    },
    "dependencies": {
        "botbuilder": "^4.1.5",
        "botframework-config": "^4.1.5",
        "dotenv": "^6.1.0",
        "restify": "^7.2.2"
    },
    "devDependencies": {
        "concurrently": "^4.0.1",
        "eslint": "^5.8.0",
        "eslint-config-standard": "^12.0.0",
        "eslint-plugin-import": "^2.14.0",
        "eslint-plugin-node": "^8.0.0",
        "eslint-plugin-promise": "^4.0.1",
        "eslint-plugin-standard": "^4.0.0",
        "nodemon": "^1.18.6",
        "@types/node": "10.12.2",
        "@types/restify": "7.2.6"
    }
}
