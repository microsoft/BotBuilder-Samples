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
        "restify": "^7.2.3"
    },
    "devDependencies": {
        "@types/restify": "7.2.6",
        "nodemon": "^1.18.6",
        "tslint": "^5.11.0",
        "typescript": "^3.1.6"
    }
}
