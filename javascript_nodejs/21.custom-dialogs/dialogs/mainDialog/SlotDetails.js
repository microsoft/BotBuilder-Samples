module.exports = class SlotDetails {
    constructor(name, promptId, prompt, reprompt) {
        this.Name = name;
        this.PromptId = promptId;
        if (prompt && reprompt) {
            this.Options = {
                prompt: prompt,
                retryPrompt: reprompt,
            }
        } else {
            this.Options = prompt;
        }
    }
}