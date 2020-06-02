// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Luis;
using Microsoft.Bot.Builder;
using LivePersonProxyAssistant.Tests.Mocks;
using LivePersonProxyAssistant.Tests.Utterances;

namespace LivePersonProxyAssistant.Tests.Utilities
{
    public class GeneralTestUtil
    {
        private static readonly Dictionary<string, IRecognizerConvert> _utterances = new Dictionary<string, IRecognizerConvert>
        {
            { GeneralUtterances.Cancel, CreateIntent(GeneralUtterances.Cancel, GeneralLuis.Intent.Cancel) },
            { GeneralUtterances.Escalate, CreateIntent(GeneralUtterances.Escalate, GeneralLuis.Intent.Escalate) },
            { GeneralUtterances.FinishTask, CreateIntent(GeneralUtterances.FinishTask, GeneralLuis.Intent.FinishTask) },
            { GeneralUtterances.GoBack, CreateIntent(GeneralUtterances.GoBack, GeneralLuis.Intent.GoBack) },
            { GeneralUtterances.Help, CreateIntent(GeneralUtterances.Help, GeneralLuis.Intent.Help) },
            { GeneralUtterances.Repeat, CreateIntent(GeneralUtterances.Repeat, GeneralLuis.Intent.Repeat) },
            { GeneralUtterances.SelectAny, CreateIntent(GeneralUtterances.SelectAny, GeneralLuis.Intent.SelectAny) },
            { GeneralUtterances.SelectItem, CreateIntent(GeneralUtterances.SelectItem, GeneralLuis.Intent.SelectItem) },
            { GeneralUtterances.SelectNone, CreateIntent(GeneralUtterances.SelectNone, GeneralLuis.Intent.SelectNone) },
            { GeneralUtterances.ShowNext, CreateIntent(GeneralUtterances.ShowNext, GeneralLuis.Intent.ShowNext) },
            { GeneralUtterances.ShowPrevious, CreateIntent(GeneralUtterances.ShowPrevious, GeneralLuis.Intent.ShowPrevious) },
            { GeneralUtterances.StartOver, CreateIntent(GeneralUtterances.StartOver, GeneralLuis.Intent.StartOver) },
            { GeneralUtterances.Stop, CreateIntent(GeneralUtterances.Stop, GeneralLuis.Intent.Stop) },
        };

        public static MockLuisRecognizer CreateRecognizer()
        {
            var recognizer = new MockLuisRecognizer(defaultIntent: CreateIntent(string.Empty, GeneralLuis.Intent.None));
            recognizer.RegisterUtterances(_utterances);
            return recognizer;
        }

        public static GeneralLuis CreateIntent(string userInput, GeneralLuis.Intent intent)
        {
            var result = new GeneralLuis
            {
                Text = userInput,
                Intents = new Dictionary<GeneralLuis.Intent, IntentScore>()
            };

            result.Intents.Add(intent, new IntentScore() { Score = 0.9 });

            result.Entities = new GeneralLuis._Entities
            {
                _instance = new GeneralLuis._Entities._Instance()
            };

            return result;
        }
    }
}
