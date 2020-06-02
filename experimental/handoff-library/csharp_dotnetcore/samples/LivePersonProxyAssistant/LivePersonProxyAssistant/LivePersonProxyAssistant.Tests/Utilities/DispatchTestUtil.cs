// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Luis;
using Microsoft.Bot.Builder;
using LivePersonProxyAssistant.Tests.Mocks;
using LivePersonProxyAssistant.Tests.Utterances;

namespace LivePersonProxyAssistant.Tests.Utilities
{
    public class DispatchTestUtil
    {
        private static Dictionary<string, IRecognizerConvert> _utterances = new Dictionary<string, IRecognizerConvert>
        {
            { GeneralUtterances.Cancel, CreateIntent(GeneralUtterances.Cancel, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.Escalate, CreateIntent(GeneralUtterances.Escalate, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.FinishTask, CreateIntent(GeneralUtterances.FinishTask, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.GoBack, CreateIntent(GeneralUtterances.GoBack, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.Help, CreateIntent(GeneralUtterances.Help, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.Repeat, CreateIntent(GeneralUtterances.Repeat, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.SelectAny, CreateIntent(GeneralUtterances.SelectAny, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.SelectItem, CreateIntent(GeneralUtterances.SelectItem, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.SelectNone, CreateIntent(GeneralUtterances.SelectNone, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.ShowNext, CreateIntent(GeneralUtterances.ShowNext, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.ShowPrevious, CreateIntent(GeneralUtterances.ShowPrevious, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.StartOver, CreateIntent(GeneralUtterances.StartOver, DispatchLuis.Intent.l_General) },
            { GeneralUtterances.Stop, CreateIntent(GeneralUtterances.Stop, DispatchLuis.Intent.l_General) },
        };

        public static MockLuisRecognizer CreateRecognizer()
        {
            var recognizer = new MockLuisRecognizer(defaultIntent: CreateIntent(string.Empty, DispatchLuis.Intent.None));
            recognizer.RegisterUtterances(_utterances);
            return recognizer;
        }

        public static DispatchLuis CreateIntent(string userInput, DispatchLuis.Intent intent)
        {
            var result = new DispatchLuis
            {
                Text = userInput,
                Intents = new Dictionary<DispatchLuis.Intent, IntentScore>()
            };

            result.Intents.Add(intent, new IntentScore() { Score = 0.9 });

            result.Entities = new DispatchLuis._Entities
            {
                _instance = new DispatchLuis._Entities._Instance()
            };

            return result;
        }
    }
}