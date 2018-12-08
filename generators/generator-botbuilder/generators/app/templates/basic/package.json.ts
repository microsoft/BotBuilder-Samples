{
    "name": "<%= botname %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Generated using Microsoft Bot Builder Yeoman generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "build": "node_modules/.bin/tsc --build",
        "start": "node_modules/.bin/tsc --build && node ./lib/index.js",
        "watch": "node_modules/.bin/nodemon --watch ./src -e ts --exec \"npm run start\"",
        "lint": "node_modules/.bin/tslint -c tslint.json 'src/**/*.ts'",
        "test": "echo \"Error: no test specified\" && exit 1"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "botbuilder": "^4.1.5",
        "botbuilder-ai": "^4.1.5",
        "botbuilder-core": "^4.1.5",
        "botbuilder-dialogs": "^4.1.5",
        "botframework-config": "^4.1.5",
        "botframework-connector": "^4.1.5",
        "botframework-schema": "^4.1.5",
        "dotenv": "^6.1.0",
        "restify": "^7.2.3"
    },
    "devDependencies": {
        "@types/dotenv": "6.1.0",
        "@types/restify": "7.2.6",
        "nodemon": "^1.18.7",
        "tslint": "^5.11.0",
        "typescript": "^3.1.6"
    }
}
