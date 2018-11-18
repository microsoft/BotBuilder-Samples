{
    "name": "<%= botName %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Microsoft Bot Builder Yeoman Generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "build": "tsc",
        "start": "tsc && node ./lib/index.js",
        "watch": "concurrently --kill-others \"tsc -w\" \"nodemon ./lib/index.js\""
        "test": "echo \"Error: no test specified\" && exit 1"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },    
    "dependencies": {
        "botbuilder": "^4.1.5",
        "botframework-config": "^4.1.5",
        "dotenv": "^6.1.0",
        "restify": "^7.2.2"
    },
    "devDependencies": {
        "concurrently": "^4.0.1",
        "nodemon": "^1.18.6",
        "tslint": "^5.11.0",
        "tslint-microsoft-contrib": "^5.2.1"
    }
}
