// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const https = require('https');

// Minimum Score For Low Score Variation
const MinimumScoreForLowScoreVariation = 20;

// Previous Low Score Variation Multiplier
const PreviousLowScoreVariationMultiplier = 1.4;

// Max Low Score Variation Multiplier
const MaxLowScoreVariationMultiplier = 2.0;

// Maximum Score For Low Score Variation
const MaximumScoreForLowScoreVariation = 95.0;

class ActiveLearningHelper{
    
    /**
    * Returns list of qnaSearch results which have low score variation.
    * @param {QnAMakerResult[]} qnaSearchResults A list of results returned from the QnA getAnswer call.
    */
    getLowScoreVariation(qnaSearchResults){
        
        if (qnaSearchResults == null || qnaSearchResults.length == 0){
            return [];
        }

        if(qnaSearchResults.length == 1){
            return qnaSearchResults;
        }

        var filteredQnaSearchResult = [];
        var topAnswerScore = qnaSearchResults[0].score * 100;
        var prevScore = topAnswerScore;

        if((topAnswerScore > MinimumScoreForLowScoreVariation) && (topAnswerScore < MaximumScoreForLowScoreVariation)){
            filteredQnaSearchResult.push(qnaSearchResults[0]);

            for(var i = 1; i < qnaSearchResults.length; i++){
                if (this.includeForClustering(prevScore, qnaSearchResults[i].score * 100, PreviousLowScoreVariationMultiplier) && this.includeForClustering(topAnswerScore, qnaSearchResults[i].score * 100, MaxLowScoreVariationMultiplier)){
                    prevScore = qnaSearchResults[i].score * 100;
                    filteredQnaSearchResult.push(qnaSearchResults[i]);
                }
            }
        }

        return filteredQnaSearchResult;
    }


    includeForClustering(prevScore, currentScore, multiplier)
    {
        return (prevScore - currentScore) < (multiplier * Math.sqrt(prevScore));
    }

    /**
     * Method to call QnAMaker Train API for Active Learning
     * @param {string} host Endpoint host of the runtime
     * @param {FeedbackRecords[]} feedbackRecords Body of the train API
     * @param {string} kbId Knowledgebase Id
     * @param {string} key Endpoint key
     */
    async callTrain(host, feedbackRecords, kbId, key){

        var data = JSON.stringify(feedbackRecords);
        
        const headers = {
            "Content-Type": "application/json",
            "Content-Length": data.length,
            "Authorization": "EndpointKey " + key
        };

        var options = {
            hostname:  host.split('/')[2],
            path: "/qnamaker/knowledgebases/" + kbId + "/train/",
            port: 443,
            method: "POST",
            headers: headers,
        }

        var req = https.request( options, (res) =>{
            res.statusCode;
        });
        
        req.write(data);
        req.end();
    }
}

module.exports.ActiveLearningHelper = ActiveLearningHelper;