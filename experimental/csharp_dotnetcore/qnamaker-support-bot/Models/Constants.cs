// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Models
{
    /// <summary>
    /// Constants for the project.
    /// </summary>
    public static class Constants
    {
        public static bool EnablePersonalityChat = false;
        public static bool EnableLuis = false;
        public static int TotalOptions = 6;
        public static int ConsecutiveNeuroconAnswersAllowed = 3;
        public static int MaxContinuousTextDialogs = 3;
        public static string WelcomeQuestion = @"Hi there! I'm the QnA Maker support bot. What can I help you with today? Just so you know, you can enter MENU anytime you want to go back to the options below or just go ahead type your query.";
        public static string[] WelcomeOptions = new string[] { "Learn more about QnA Maker", "I am having a problem", "Give feedback" };
        public static string HeroCardTitle = string.Empty;
        public static int DefaultTop = 1;
        public static int ActiveLearningTop = 3;
        public static float DefaultThreshold = 0.6F;
        public static float ActiveLearningThreshold = 0.6F;
        public static float MultiturnThreshold = 0.85F;
        public static string NeuroconRedirectionQuestion = "redirection Neurocon";
        public static string GuidedFlowRedirectionQuestion = "redirection GuidedFlow";
        public static string PersonalityChatKey = string.Empty;
        public static string EventWelcomeMessage = "EventWelcomeMessage";

        public enum RedirectionType
        {
            Neurocon,
            GuidedFlow,
            RedirectionIntent
        }

        public struct MetadataValue
        {
            public static string ExcludeIsroot = "no";
            public static string Welcome = "welcome message";
            public static string Redirection = "multiturn end";
            public static string Feedback = "feedback";
            public static string ChitChat = "chitchat";
        }

        public struct MetadataName
        {
            public static string Requery = "requery";
            public static string Option = "option";
            public static string Parent = "parent";
            public static string Name = "name";
            public static string Isroot = "isroot";
            public static string Intent = "intent";
            public static string Flowtype = "flowtype";
            public static string OptionRequery = "optionrequery";
            public static string Editorial = "editorial";
            public static string ActiveLearning = "suggestion";
            public static string QnAId = "qnaid";
        }

        /// <summary>
        /// This is for handling capitalization while showing options.
        /// </summary>
        public struct FormattedWord
        {
            public string Original;
            public string ConvertTo;
        }
    }
}
