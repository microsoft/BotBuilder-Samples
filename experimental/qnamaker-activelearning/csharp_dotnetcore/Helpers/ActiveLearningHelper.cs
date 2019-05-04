using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Newtonsoft.Json;

namespace QnAMakerActiveLearningBot.Helpers
{
    public static class ActiveLearningHelper
    {
        /// <summary>
        /// Minimum Score For Low Score Variation
        /// </summary>
        private const double MinimumScoreForLowScoreVariation = 20.0;

        /// <summary>
        /// Previous Low Score Variation Multiplier
        /// </summary>
        private const double PreviousLowScoreVariationMultiplier = 1.4;

        /// <summary>
        /// Max Low Score Variation Multiplier
        /// </summary>
        private const double MaxLowScoreVariationMultiplier = 2.0;


        /// <summary>
        /// Maximum Score For Low Score Variation
        /// </summary>
        private const double MaximumScoreForLowScoreVariation = 95.0;

        /// <summary>
        /// Returns list of qnaSearch results which have low score variation
        /// </summary>
        /// <param name="qnaSearchResults">List of QnaSearch results</param>
        /// <returns>List of filtered qnaSearch results</returns>
        public static List<QueryResult> GetLowScoreVariation(List<QueryResult> qnaSearchResults)
        {
            var filteredQnaSearchResult = new List<QueryResult>();

            if (qnaSearchResults == null || qnaSearchResults.Count == 0)
            {
                return filteredQnaSearchResult;
            }

            if (qnaSearchResults.Count == 1)
            {
                return qnaSearchResults;
            }

            var topAnswerScore = qnaSearchResults[0].Score * 100;
            var prevScore = topAnswerScore;

            if ((topAnswerScore > MinimumScoreForLowScoreVariation) && (topAnswerScore < MaximumScoreForLowScoreVariation))
            {
                filteredQnaSearchResult.Add(qnaSearchResults[0]);

                for (var i = 1; i < qnaSearchResults.Count; i++)
                {
                    if (IncludeForClustering(prevScore, qnaSearchResults[i].Score * 100, PreviousLowScoreVariationMultiplier) && IncludeForClustering(topAnswerScore, qnaSearchResults[i].Score * 100, MaxLowScoreVariationMultiplier))
                    {
                        prevScore = qnaSearchResults[i].Score * 100;
                        filteredQnaSearchResult.Add(qnaSearchResults[i]);
                    }
                }
            }
            
            return filteredQnaSearchResult;
        }

        private static bool IncludeForClustering(double prevScore, double currentScore, double multiplier)
        {
            return (prevScore - currentScore) < (multiplier * Math.Sqrt(prevScore));
        }

        /// <summary>
        /// Method to call QnAMaker Train API for Active Learning
        /// </summary>
        /// <param name="host">Endpoint host of the runtime</param>
        /// <param name="FeedbackRecords">Feedback records train API</param>
        /// <param name="kbId">Knowledgebase Id</param>
        /// <param name="key">Endpoint key</param>
        /// <param name="cancellationToken"> Cancellation token</param>
        public async static void CallTrain(string host, FeedbackRecords feedbackRecords, string kbId, string key, CancellationToken cancellationToken)
        {
            var uri = host + "/knowledgebases/" + kbId + "/train/";

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(JsonConvert.SerializeObject(feedbackRecords), Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "EndpointKey " + key);

                    var response = await client.SendAsync(request, cancellationToken);
                    await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
