// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const request = require('request-promise-native');

class QnAServiceHelper {
    
    static async queryQnAService(query, qnAcontext) {

        const endpoint = process.env.QnAEndpointHostName;
        const kbId = process.env.QnAKnowledgebaseId;
        const key = process.env.QnAAuthKey;

        const url =  `${ endpoint }/qnamaker/knowledgebases/${ kbId }/generateanswer`;
        const headers = {
            "Content-Type": "application/json",
            "Authorization": "EndpointKey " + key
        };

        if (qnAcontext == null){
            qnAcontext = {
                PreviousQnaId: 0,
                PreviousUserQuery: null
            }
        }
        
        const qnaResult = await request({
            url: url,
            method: 'POST',
            headers: headers,
            json: {
                question: query,
                top: 3,
                context: qnAcontext,
            }
        });

        return qnaResult.answers;
    }
}

module.exports.QnAServiceHelper = QnAServiceHelper;
