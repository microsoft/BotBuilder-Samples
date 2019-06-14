// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Steps
{
    public class Search : DialogCommand
    {
        public Search()
        {
            var Search = new AdaptiveDialog(nameof(AdaptiveDialog)) {
                Steps = new List<IDialog>() {
                    new SetProperty(){
                        Property = "user.firstName",
                        Value = null
                    }

                }

            };

        }

        protected override Task<DialogTurnResult> OnRunCommandAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
