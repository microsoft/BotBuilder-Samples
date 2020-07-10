// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Luis;
using Microsoft.Bot.Builder;
using TaskModuleFactorySample.Tests.Mocks;
using TaskModuleFactorySample.Tests.Utterances;

namespace TaskModuleFactorySample.Tests.Utilities
{
    public class SkillTestUtil
    {
        private static Dictionary<string, IRecognizerConvert> _utterances = new Dictionary<string, IRecognizerConvert>
        {
            { SampleDialogUtterances.Trigger, CreateIntent(SampleDialogUtterances.Trigger, TaskModuleFactorySampleLuis.Intent.Sample) },
        };

        public static MockLuisRecognizer CreateRecognizer()
        {
            var recognizer = new MockLuisRecognizer(defaultIntent: CreateIntent(string.Empty, TaskModuleFactorySampleLuis.Intent.None));
            recognizer.RegisterUtterances(_utterances);
            return recognizer;
        }

        public static TaskModuleFactorySampleLuis CreateIntent(string userInput, TaskModuleFactorySampleLuis.Intent intent)
        {
            var result = new TaskModuleFactorySampleLuis
            {
                Text = userInput,
                Intents = new Dictionary<TaskModuleFactorySampleLuis.Intent, IntentScore>()
            };

            result.Intents.Add(intent, new IntentScore() { Score = 0.9 });

            result.Entities = new TaskModuleFactorySampleLuis._Entities
            {
                _instance = new TaskModuleFactorySampleLuis._Entities._Instance()
            };

            return result;
        }
    }
}
