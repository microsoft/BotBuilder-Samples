// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ConversationState, UserState } = require('botbuilder');
const { Dialog, DialogSet } = require('botbuilder-dialogs');
const fs = require('fs');
const path = require('path');

class BotRunner extends ActivityHandler {
    /**
     * This class creates a specialized ActivityHandler that will call the provided main dialog for all incoming messages.
     * It will also continue to emit other events which can be handled using the respective on<Event> methods.
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     * @param {Dialog} dialog
     * @param {any} logger object for logging events, defaults to console if none is provided
     */
    constructor(conversationState, userState, dialog, logger) {
        super();
        if (!conversationState) throw new Error('[BotRunner]: Missing parameter. conversationState is required');
        if (!userState) throw new Error('[BotRunner]: Missing parameter. userState is required');
        if (!dialog) throw new Error('[BotRunner]: Missing parameter. dialog is required');
        if (!logger) {
            logger = console;
            logger.log('[BotRunner]: logger not passed in, defaulting to console');
        }
        
        this.conversationState = conversationState;
        this.userState = userState;
        this.dialog = dialog;
        this.logger = logger;
        this.dialogState = this.conversationState.createProperty('DialogState');
        this.dialogSet = new DialogSet(this.dialogState);
        this.dialogSet.add(this.dialog);

        // onDialog fires after onMessage and other message-related events
        this.onDialog(async context => {
            this.logger.log('Running dialog with Message Activity.');

            // Create a DialogContext in the top level dialogSet...
            const dialogContext = await this.dialogSet.createContext(context);

            // Run the Dialog with the new message Activity.
            await this.dialog.run(dialogContext);
    
            // Save any state changes. The load happened during the execution of the Dialog. 
            await this.conversationState.saveChanges(context, false);
            await this.userState.saveChanges(context, false);
        });

    }

    /**
     * Load a Javascript module, then execute its main function with `this` as a paramter
     * @param {*} p Path to a specific handler module
     */
    loadModule(p) {
        this.logger.log('Load Module:',p);
        require(p)(this);
    }

    /**
     * Automatically load all the Javascript modules plugins in a given path
     * Modules should be in the form module.exports = function(bot) { ... }
     * @param {*} p Path to handler modules 
     */
    loadModules(p) {
        // load all the .js files from this path
        fs.readdirSync(p).filter((f) => { return (path.extname(f) === '.js'); }).forEach((file) => {
            this.loadModule(path.join(p,file));
        });
    }        

    
}

module.exports.BotRunner = BotRunner;
