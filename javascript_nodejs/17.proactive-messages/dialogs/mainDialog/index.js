// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TurnContext } = require('botbuilder');

class MainDialog {
    /**
     * 
     * @param {Storage} storage A storage system like MemoryStorage used to store information.
     * @param {BotAdapter} adapter A BotAdapter used to send and receive messages.
     */
    constructor (storage, adapter) {
        this.storage = storage;
        this.adapter = adapter;
    }

    /**
     * 
     * @param {TurnContext} context A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {

            const utterance = (context.activity.text || '').trim().toLowerCase();

            // If user types in run, create a new job
            if (utterance == "run"){
                await this.createJob(this.storage, context);
            }

            const firstWord = utterance.split(' ')[0];
            const secondWord = utterance.split(' ')[1];

            // If the user types done and a Job Id Number
            // We check if the second word input is a number
            if (firstWord == "done" && !isNaN(parseInt(secondWord))) {
                var jobIDNumber = secondWord;
                await this.completeJob(this.storage, context, jobIDNumber);
            } else if (firstWord == "done" && isNaN(parseInt(secondWord))) {
                await context.sendActivity('Enter the job ID number after "done".');
            }

            if (!context.responded) {
                await context.sendActivity(`Say "run" to start a job, or "done <job>" to complete one.`);
            }

        } else if (context.activity.type === 'event' && context.activity.name === 'jobCompleted') {
            var jobIDNumber = context.activity.value;
            if (!isNaN(parseInt(jobIDNumber))) {
                await this.completeJob(this.storage, context, jobIDNumber);
            }
        }
    }

    // Save job ID and conversation reference
    async createJob(storage, context) {

        // Create a unique job ID
        var date = new Date();
        var jobIDNumber = date.getTime();

        //Get the conversation reference
        const reference = TurnContext.getConversationReference(context.activity);

        // Try to find previous information about the saved job:
        // This object will never be true since we are passing in a unique jobID
        let jobInfo = await storage.read([jobIDNumber]);

        try{
            if(isEmpty(jobInfo)){
                // Job object is empty so we have to create it
                await context.sendActivity(`need to create new job ID: ${jobIDNumber}`);

                // Update jobInfo with new info
                jobInfo[jobIDNumber] = {completed: false, reference: reference};

                try {
                    // Save to storage
                    await storage.write(jobInfo)
                    // Notify the user that the job has been processed 
                    await context.sendActivity('Successful write to log.');
                } catch (err) {
                    await context.sendActivity(`Write failed: ${err}`);
                }
            }
        } catch (err){
            await context.sendActivity(`Read rejected. ${err}`);
        }
    }

    async completeJob(storage, context, jobIDNumber){

        // Read from storage
        let jobInfo = await storage.read([jobIDNumber]);

        // If no job notify the user
        if(isEmpty(jobInfo)){
            await context.sendActivity(`Sorry no job with ID ${jobIDNumber}`);
        }else {

            // Found a job with the ID passed in
            const reference = jobInfo[jobIDNumber].reference;
            const completed = jobInfo[jobIDNumber].completed;

            // If not completed and reference exists 
            if(reference && !completed){
                // Since activity was not received it has to be created.
                await this.adapter.continueConversation(reference, async (context) => {
                    // Complete the job
                    jobInfo[jobIDNumber].completed = true;
                    // Save the updated job
                    await storage.write(jobInfo);
                    // Notify the user that the job is complete
                    await context.sendActivity('Job complete');
                })
            }
            // The job has already been completed
            else if(reference && completed){
                await this.adapter.continueConversation(reference, async (context) => {
                    await context.sendActivity('This job is already completed, please start a new job');
                })
            };
        };
    };

}

// Helper function to check if object is empty
function isEmpty(obj) {
    for(var key in obj) {
        if(obj.hasOwnProperty(key))
            return false;
    }
    return true;
};

module.exports = MainDialog;