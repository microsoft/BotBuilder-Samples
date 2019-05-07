// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory } = require('botbuilder');
const { QnAServiceHelper } = require('../helpers/qnAServiceHelper');
const { CardHelper} = require('../helpers/cardHelper');
const {FunctionDialogBase} = require('./functionDialogBase');

class QnADialog extends FunctionDialogBase {
    
    constructor() {
        super('qnaDialog');
    }

    async processAsync(oldState, activity){

        var newState = null;
        var query = activity.text;
        var qnaResult = await QnAServiceHelper.queryQnAService(query, oldState);
        var qnaAnswer = qnaResult[0].answer;

        var prompts = null;
        if(qnaResult[0].context != null){
            prompts = qnaResult[0].context.prompts;
        }
        
        var outputActivity = null;
        if(prompts == null || prompts.length < 1){
            outputActivity = MessageFactory.text(qnaAnswer);
        }
        else{
            var newState = {
                PreviousQnaId: qnaResult[0].id,
                PreviousUserQuery: query
            }

            outputActivity = CardHelper.GetHeroCard(qnaAnswer, prompts);
        }
        
        return [newState, outputActivity , null];
    }  
}

module.exports.QnADialog = QnADialog;
