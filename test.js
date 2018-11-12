 /**
  * WaterfallDialogStep to process the user's picture.
  * @param {WaterfallStepContext} step WaterfallStepContext
  */
 async processPhotoStep(step) {
     await this.writeLogInTheStorage('Start downloading picture....');
     await this.handleIncomingAttachment(step);
     return await step.endDialog();
 };
 /**
  * responds to the user with information about the saved attachment or an error.
  * @param {Object} turnContext 
  */
 async handleIncomingAttachment(step) {
     // Prepare Promises to download each attachment and then execute each Promise.
     const attachment = step.context.activity.attachments[0];
     const tokenIsRequired = await this.checkRequiresToken(step.context);
     const dc = await this.dialogs.createContext(step.context);
     const token = await dc.beginDialog(LOGIN_PROMPT); //await step.context.adapter.getUserToken(step.context, CONNECTION_SETTING_NAME);
     let file = undefined;
     if (tokenIsRequired) {
         file = await this.downloadAttachment(token.result.token, attachment.contentUrl);
     } else {
         file = await requestX(attachment.contentUrl);
     }
     await OAuthHelpers.postPhoto(step.context, token.result, file);
 }

 async downloadAttachment(token, url) {
     const p = new Promise((resolve, reject) => {
         request({
             url: url,
             headers: {
                 'Authorization': 'Bearer ' + token,
                 'Content-Type': 'application/octet-stream'
             }
         }, async function(err, response, body) {
             const result = body

             if (err) {
                 console.log(err);
                 //await this.writeLogInTheStorage('err 1 : ' + err);
                 reject(err);
             } else if (result.error) {
                 console.log(result.error);
                 //await this.writeLogInTheStorage('err 2 : ' + err);
                 reject(result.error.message);
             } else {
                 // The value of the body will be an array.
                 console.log(result);
                 //await this.writeLogInTheStorage('success : ' + result);
                 resolve(result);
             }
         });
     });
     return p;
 }