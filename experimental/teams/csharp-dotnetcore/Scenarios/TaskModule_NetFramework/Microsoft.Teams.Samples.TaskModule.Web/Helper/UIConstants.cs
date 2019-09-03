// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Microsoft.Teams.Samples.TaskModule.Web.Helper
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