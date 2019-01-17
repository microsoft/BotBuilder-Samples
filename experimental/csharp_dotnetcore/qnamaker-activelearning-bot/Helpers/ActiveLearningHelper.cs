using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;

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
        private const double PreviousLowScoreVariationMultiplier = 0.8;

        /// <summary>
        /// Max Low Score Variation Multiplier
        /// </summary>
        private const double MaxLowScoreVariationMultiplier = 1.6;

        /// <summary>
        /// Max Score For Single Answer Suggestion
        /// </summary>
        private const double MaxScoreForSingleAnswerSuggestion = 50;

        /// <summary>
        /// Returns list of qnaSearch results which have low score variation
        /// </summary>
        /// <param name="qnaSearchResults">List of QnaSearch results</param>
        /// <returns>List of filtered qnaSearch results</returns>
        public static List<QueryResult> GetLowScoreVariation(List<QueryResult> qnaSearchResults)
        {
            var filteredQnaSearchResult = new List<QueryResult>();

            if (qnaSearchResults == null)
            {
                return filteredQnaSearchResult;
            }

            if (qnaSearchResults.Count > 0)
            {
                if (qnaSearchResults.Count == 1 && IsSingleAnswerSuggestion(qnaSearchResults[0].Score))
                {
                    filteredQnaSearchResult.Add(qnaSearchResults[0]);
                    return filteredQnaSearchResult;
                }

                var topAnswerScore = qnaSearchResults[0].Score * 100;
                var prevScore = topAnswerScore;

                if (topAnswerScore > MinimumScoreForLowScoreVariation)
                {
                    filteredQnaSearchResult.Add(qnaSearchResults[0]);

                    for (var i = 1; i < qnaSearchResults.Count; i++)
                    {
                        if (IncludeForClustering(prevScore, qnaSearchResults[i].Score * 100, PreviousLowScoreVariationMultiplier) && IncludeForClustering(topAnswerScore, qnaSearchResults[i].Score * 100, MaxLowScoreVariationMultiplier))
                        {
                            prevScore = qnaSearchResults[i].Score;
                            filteredQnaSearchResult.Add(qnaSearchResults[i]);
                        }
                    }
                }
            }

            return filteredQnaSearchResult;
        }

        private static bool IsSingleAnswerSuggestion(double score)
        {
            return (score < MaxScoreForSingleAnswerSuggestion) ? true : false;
        }

        private static bool IncludeForClustering(double prevScore, double currentScore, double multiplier)
        {
            return (prevScore - currentScore) < multiplier * Math.Sqrt(prevScore);
        }

        /// <summary>
        /// Method ot call train API
        /// </summary>
        /// <param name="host">Endpoint host of the runtime</param>
        /// <param name="body">Body of the train API</param>
        /// <param name="kbId">Knowledgebase Id</param>
        /// <param name="key">Endpoint key</param>
        public async static void CallTrain(string host, string body, string kbId, string key)
        {
            var uri = host + "/knowledgebases/" + kbId + "/train/";

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "EndpointKey " + key);

                    var response = await client.SendAsync(request);
                    await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
