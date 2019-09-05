// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TeamsTasksBot.Helper
{

    public static class TaskModuleUIConstants
    {
        public static UIConstants YouTube { get; set; } =
            new UIConstants(1000, 700, "Microsoft Ignite 2018 Vision Keynote", TaskModuleIds.YouTube, "YouTube");
        public static UIConstants PowerApp { get; set; } =
            new UIConstants(720, 520, "PowerApp: Asset Checkout", TaskModuleIds.PowerApp, "Power App");
        public static UIConstants CustomForm { get; set; } =
            new UIConstants(510, 450, "Custom Form", TaskModuleIds.CustomForm, "Custom Form");
        public static UIConstants AdaptiveCard { get; set; } =
            new UIConstants(700, 500, "Adaptive Card: Inputs", TaskModuleIds.AdaptiveCard, "Adaptive Card");
    }

    public class UIConstants
    {
        public UIConstants(int width, int height, string title, string id, string buttonTitle)
        {
            Width = width;
            Height = height;
            Title = title;
            Id = id;
            ButtonTitle = buttonTitle;
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public string Title { get; set; }
        public string ButtonTitle { get; set; }
        public string Id { get; set; }
    }

    public class TaskModuleIds
    {
        public const string YouTube = "youtube";
        public const string PowerApp = "powerapp";
        public const string CustomForm = "customform";
        public const string AdaptiveCard = "adaptivecard";
    }
}