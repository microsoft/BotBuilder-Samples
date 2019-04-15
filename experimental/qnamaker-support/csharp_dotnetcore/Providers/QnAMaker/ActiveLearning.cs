// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.AI.QnA;
    using Newtonsoft.Json;
    using SupportBot.Models;

    public class ActiveLearning
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

        /// <summary>
        /// Call tarin API.
        /// </summary>
        /// <param name="activeLearningData">Active Learning Data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async static void CallTrainApi(ActiveLearningDTO activeLearningData, CancellationToken cancellationToken = default(CancellationToken))
        {
            var records = new FeedbackRecord[]
            {
                 new FeedbackRecord()
                {
                    QnaId = activeLearningData.qnaId,
                    UserQuestion = activeLearningData.userQuestion,
                },
            };
            var feedbackRecords = new FeedbackRecords { Records = records };

            var uri = activeLearningData.hostName + "/knowledgebases/" + activeLearningData.kbid + "/train/";

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(uri);
                        request.Content = new StringContent(JsonConvert.SerializeObject(feedbackRecords), Encoding.UTF8, "application/json");
                        request.Headers.Add("Authorization", "EndpointKey " + activeLearningData.endpointKey);

                        var response = await client.SendAsync(request, cancellationToken);
                        await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception)
            {
                // TODO : Log exception
            }
        }

        /// <summary>
        /// Returns Active learning response
        /// </summary>
        /// <param name="responseCandidates">List of QnaSearch results</param>
        /// <returns>Array of filtered qnaSearch results</returns>
        public static QueryResult GenerateResponse(List<QueryResult> responseCandidates)
        {
            var finalResponse = new QueryResult();
            var finalMetadatas = new List<Metadata>();
            var count = 1;
            foreach (var response in responseCandidates)
            {
                finalResponse.Answer = "Do you mean : ";
                var optionPrompt = response.Metadata.Where(m => m.Name.Equals(Constants.MetadataName.ActiveLearning, StringComparison.OrdinalIgnoreCase));
                string question = null;
                if (optionPrompt != null && optionPrompt.Count() != 0 && optionPrompt.First().Value != null)
                {
                    question = optionPrompt.First().Value;
                }

                var metadataOption = new Metadata()
                {
                    Name = Constants.MetadataName.Option + count.ToString(),
                    Value = string.IsNullOrEmpty(question) ? response.Questions[0] : question,
                };
                finalMetadatas.Add(metadataOption);
                var requeryMetadata = response.Metadata.Where(m => m.Name.Equals(Constants.MetadataName.Requery + count.ToString(), StringComparison.OrdinalIgnoreCase));
                if (requeryMetadata != null && requeryMetadata.Count() != 0 && requeryMetadata.First().Value != null)
                {
                    var metadataRequery = new Metadata()
                    {
                        Name = Constants.MetadataName.Requery + count.ToString(),
                        Value = requeryMetadata.First().Value,
                    };
                    finalMetadatas.Add(metadataRequery);
                }

                var metadataQnAId = new Metadata()
                {
                    Name = Constants.MetadataName.QnAId + count.ToString(),
                    Value = response.Id.ToString(),
                };
                finalMetadatas.Add(metadataQnAId);

                count++;
            }

            finalResponse.Metadata = finalMetadatas.ToArray();
            return finalResponse;
        }

        private static bool IncludeForClustering(double prevScore, double currentScore, double multiplier)
        {
            return (prevScore - currentScore) < (multiplier * Math.Sqrt(prevScore));
        }
    }
}
