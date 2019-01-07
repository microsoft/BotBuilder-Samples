{
<<<<<<< HEAD
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
        "watch": "tsc && node ./lib/index.js"
      },
    "dependencies": {
        "botbuilder": "^4.0.6",
        "botframework-config": "^4.0.6",
        "dotenv": "^6.0.0",
        "restify": "^6.3.4"
    },
    "devDependencies": {
        "eslint": "^5.6.0",
        "eslint-config-standard": "^12.0.0",
        "eslint-plugin-import": "^2.14.0",
        "eslint-plugin-node": "^7.0.1",
        "eslint-plugin-promise": "^4.0.1",
        "eslint-plugin-standard": "^4.0.0",
        "nodemon": "^1.18.4",
        "@types/node": "10.10.2",
        "@types/restify": "7.2.4"
=======
    "name": "<%= botname %>",
    "version": "1.0.0",
    "description": "<%= botDescription %>",
    "author": "Generated using Microsoft Bot Builder Yeoman generator v<%= version %>",
    "license": "MIT",
    "main": "<%= npmMain %>",
    "scripts": {
        "build": "node_modules/.bin/tsc --build",
        "lint": "node_modules/.bin/tslint -c tslint.json 'src/**/*.ts'",
        "postinstall": "npm run build && node ./deploymentScripts/webConfigPrep.js",
        "start": "node_modules/.bin/tsc --build && node ./lib/index.js",
        "test": "echo \"Error: no test specified\" && exit 1",
        "watch": "node_modules/.bin/nodemon --watch ./src -e ts --exec \"npm run start\""
    },
    "repository": {
        "type": "git",
        "url": "https://github.com"
    },
    "dependencies": {
        "botbuilder": "^4.2.0",
        "botframework-config": "^4.2.0",
        "dotenv": "^6.1.0",
        "replace": "^1.0.0",
        "restify": "^7.2.3"
    },
    "devDependencies": {
        "@types/dotenv": "6.1.0",
        "@types/restify": "7.2.6",
        "nodemon": "^1.18.7",
        "tslint": "^5.11.0",
        "typescript": "^3.1.6"
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
}
