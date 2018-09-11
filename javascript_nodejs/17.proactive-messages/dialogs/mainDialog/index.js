// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, TurnContext } = require('botbuilder');

class MainDialog {
    /**
     * 
     * @param {Storage} storage A storage system like MemoryStorage used to store information.
     * @param {BotAdapter} adapter A BotAdapter used to send and receive messages.
     */
    constructor(storage, adapter) {
        this.storage = storage;
        this.adapter = adapter;
    }

    /**
     * 
     * @param {TurnContext} turnContext A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {

            const utterance = (turnContext.activity.text || '').trim().toLowerCase();

            // If user types in run, create a new job.
            if (utterance === "run"){
                await this.createJob(this.storage, turnContext);
            }

            const firstWord = utterance.split(' ')[0];
            const secondWord = utterance.split(' ')[1];

            // If the user types done and a Job Id Number,
            // we check if the second word input is a number.
            if (firstWord === "done" && !isNaN(parseInt(secondWord))) {
                var jobIdNumber = secondWord;
                await this.completeJob(this.storage, turnContext, jobIdNumber);
                await turnContext.sendActivity('Job completed. Notification sent.');

            } else if (firstWord === "done" && isNaN(parseInt(secondWord))) {
                await turnContext.sendActivity('Enter the job ID number after "done".');
            }

            if (!turnContext.responded) {
                await turnContext.sendActivity(`Say "run" to start a job, or "done <job>" to complete one.`);
            }

        } else if (turnContext.activity.type === 'event' && turnContext.activity.name === 'jobCompleted') {
            var jobIdNumber = turnContext.activity.value;
            if (!isNaN(parseInt(jobIdNumber))) {
                await this.completeJob(this.storage, turnContext, jobIdNumber);
            }
        }
    }

    // Save job ID and conversation reference.
    async createJob(storage, turnContext) {

        // Create a unique job ID.
        var date = new Date();
        var jobIdNumber = date.getTime();

        // Get the conversation reference.
        const reference = TurnContext.getConversationReference(turnContext.activity);

        // Try to find previous information about the saved job:
        const jobInfo = await storage.read([jobIdNumber]);

        try {
            if (isEmpty(jobInfo)){
                // Job object is empty so we have to create it
                await turnContext.sendActivity(`Need to create new job ID: ${ jobIdNumber }`);

                // Update jobInfo with new info
                jobInfo[jobIdNumber] = { completed: false, reference: reference };

                try {
                    // Save to storage
                    await storage.write(jobInfo)
                    // Notify the user that the job has been processed 
                    await turnContext.sendActivity('Successful write to log.');
                } catch(err) {
                    await turnContext.sendActivity(`Write failed: ${ err.message }`);
                }
            }
        } catch(err){
            await turnContext.sendActivity(`Read rejected. ${ err.message }`);
        }
    }

    async completeJob(storage, turnContext, jobIdNumber) {
        // Read from storage
        let jobInfo = await storage.read([jobIdNumber]);

        // If no job was found, notify the user of this error state.
        if (isEmpty(jobInfo)){
            await turnContext.sendActivity(`Sorry no job with ID ${ jobIdNumber }.`);
        } else {
            // Found a job with the ID passed in.
            const reference = jobInfo[jobIdNumber].reference;
            const completed = jobInfo[jobIdNumber].completed;

            // If the job is not yet completed and conversation reference exists,
            // use the adapter to continue the conversation with the job's original creator.
            if (reference && !completed) {
                // Since we are going to proactively send a message to the user who started the job,
                // we need to create the turnContext based on the stored reference value.
                await this.adapter.continueConversation(reference, async (turnContext) => {
                    // Complete the job.
                    jobInfo[jobIdNumber].completed = true;
                    // Save the updated job.
                    await storage.write(jobInfo);
                    // Notify the user that the job is complete.
                    await turnContext.sendActivity('Your queued job just completed.');
                });
            }
            // The job has already been completed.
            else if (reference && completed) {
                await this.adapter.continueConversation(reference, async (turnContext) => {
                    await turnContext.sendActivity('This job is already completed, please start a new job.');
                });
            };
        };
    };
}

// Helper function to check if object is empty.
function isEmpty(obj) {
    for(var key in obj) {
        if(obj.hasOwnProperty(key))
            return false;
    }
    return true;
};

module.exports = MainDialog;