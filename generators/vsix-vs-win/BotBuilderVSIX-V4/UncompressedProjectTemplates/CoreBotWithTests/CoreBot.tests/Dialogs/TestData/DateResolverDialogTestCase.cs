// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace $ext_safeprojectname$.Tests.Dialogs.TestData
{
    /// <summary>
    /// A class to store test case data for <see cref="DateResolverDialogTests"/>
    /// </summary>
    public class DateResolverDialogTestCase
    {
        public string Name { get; set; }

        public string InitialData { get; set; }

        public string[,] UtterancesAndReplies { get; set; }

        public string ExpectedResult { get; set; }
    }
}
