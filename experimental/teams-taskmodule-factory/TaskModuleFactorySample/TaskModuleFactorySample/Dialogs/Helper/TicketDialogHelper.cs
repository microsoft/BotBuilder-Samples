using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using TaskModuleFactorySample.Extensions.Teams;
using TaskModuleFactorySample.Extensions.Teams.TaskModule;
using TaskModuleFactorySample.Models;

namespace TaskModuleFactorySample.Dialogs.Helper
{
    /// <summary>
    /// Helper class to create adaptive cards
    /// </summary>
    public class TicketDialogHelper
    {
        public static AdaptiveCard CreateFormAdaptiveCard(string botId = null)
        {
            // Json Card for creating Form
            // TODO: Replace with Cards.Lg and responses
            AdaptiveCard adaptiveCard = AdaptiveCardHelper.GetCardFromJson("Dialogs/Resources/SampleForm.json");
            adaptiveCard.Id = "GetUserInput";
            adaptiveCard.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "SubmitSample",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.CreateSample_Form.ToString(),
                        Submit = true
                    }
                }
            });

            return adaptiveCard;
        }

        public static AdaptiveCard UpdateFormCardSubmit(SampleForm details, string botId = null)
        {
            var card = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveColumnSet
                            {
                                Columns = new List<AdaptiveColumn>
                                {
                                    new AdaptiveColumn
                                    {
                                        Width = AdaptiveColumnWidth.Stretch,
                                        Items = new List<AdaptiveElement>
                                        {
                                            new AdaptiveTextInput
                                            {
                                                // Get Title
                                                Placeholder = $"Title: {details.Title}",
                                                Spacing = AdaptiveSpacing.Small
                                            },
                                            new AdaptiveTextInput
                                            {
                                                // Get Urgency
                                                Placeholder = $"Urgency: {details.Urgency}",
                                                Spacing = AdaptiveSpacing.Small
                                            },
                                            new AdaptiveTextInput
                                            {
                                                // Get Description
                                                Placeholder = $"Description: {details.Description}",
                                                Spacing = AdaptiveSpacing.Small,
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            card.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "Update Form",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.UpdateSample_Form.ToString(),
                        Submit = true
                    }
                }
            });
            card.Id = "UpdateAdaptiveCard";

            return card;
        }

        public static AdaptiveCard UpdateFormCard(SampleForm details, string botId = null)
        {
            var card = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveColumnSet
                            {
                                Columns = new List<AdaptiveColumn>
                                {
                                    new AdaptiveColumn
                                    {
                                        Width = AdaptiveColumnWidth.Stretch,
                                        Items = new List<AdaptiveElement>
                                        {
                                            new AdaptiveTextBlock
                                            {
                                                // Get Title
                                                Text = $"Title: {details.Title}",
                                                Wrap = true,
                                                Spacing = AdaptiveSpacing.Small,
                                                Weight = AdaptiveTextWeight.Bolder
                                            },
                                            new AdaptiveTextBlock
                                            {
                                                // Get Urgency
                                                Text = $"Urgency: {details.Urgency}",
                                                Color = AdaptiveTextColor.Good,
                                                MaxLines = 1,
                                                Weight = AdaptiveTextWeight.Bolder,
                                                Size = AdaptiveTextSize.Large
                                            },
                                            new AdaptiveTextBlock
                                            {
                                                // Get Description
                                                Text = $"Description: {details.Description}",
                                                Wrap = true,
                                                Spacing = AdaptiveSpacing.Small,
                                                Weight = AdaptiveTextWeight.Bolder
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            card.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "Update Form",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.UpdateSample_Form.ToString(),
                        FlowData = details
                    }
                }
            });
            card.Id = "UpdateAdaptiveCard";

            return card;
        }

        // <returns> Adaptive Card.</returns>
        public static AdaptiveCard GetUserInputFormCard(string botId = null)
        {
            var card = new AdaptiveCard("1.0");

            var columns = new List<AdaptiveColumn>
            {
                new AdaptiveColumn
                {
                    VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                    Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = "Please Click Create Ticket To Create New Form",
                                Size = AdaptiveTextSize.Small,
                                Weight = AdaptiveTextWeight.Bolder,
                                Color = AdaptiveTextColor.Accent,
                                Wrap = true
                            }
                        },
                }
            };

            var columnSet = new AdaptiveColumnSet
            {
                Columns = columns,
                Separator = true
            };

            var list = new List<AdaptiveElement>
            {
                columnSet
            };

            card.Body.AddRange(list);
            card?.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "Create Ticket",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.CreateSample_Form.ToString(),
                    }
                }
            });

            return card;
        }

        /// <summary>
        /// Returns Card to GetForm Id from User.
        /// </summary>
        /// <returns> Adaptive Card.</returns>
        public static AdaptiveCard GetDeleteConfirmationCard(string ticketId, string botId = null)
        {
            var card = new AdaptiveCard("1.0");
            var columns = new List<AdaptiveColumn>
            {
                new AdaptiveColumn
                {
                    VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                    Items = new List<AdaptiveElement>
                    {
                        new AdaptiveTextBlock
                        {
                            Text = $"Deleting Ticket with id: {ticketId}",
                            Size = AdaptiveTextSize.Small,
                            Weight = AdaptiveTextWeight.Bolder,
                            Color = AdaptiveTextColor.Accent,
                            Wrap = true
                        },
                        new AdaptiveTextBlock
                        {
                            Text = $"Close Reason:",
                            Size = AdaptiveTextSize.Small,
                            Weight = AdaptiveTextWeight.Bolder,
                            Color = AdaptiveTextColor.Accent,
                            Wrap = true
                        },
                        new AdaptiveTextInput
                        {
                            Placeholder = "Enter Your Reason",
                            Id = "FormCloseReason",
                            Spacing = AdaptiveSpacing.Small,
                            IsMultiline = true
                        },
                    }
                }
            };

            var columnSet = new AdaptiveColumnSet
            {
                Columns = columns,
                Separator = true
            };

            var list = new List<AdaptiveElement>
            {
                columnSet
            };

            card.Body.AddRange(list);
            card?.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "Confirm",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.DeleteSample_Form.ToString(),
                        FlowData = new Dictionary<string, object>
                        {
                            { "FormDetails", ticketId }
                        },
                        Submit = true
                    }
                }
            });
            card.Id = "DeleteTicketAdaptiveCard";

            return card;
        }

        /// <returns>Adaptive Card.</returns>
        public static AdaptiveCard FormResponseCard(string trackerResponse)
        {
            var card = new AdaptiveCard("1.0");
            card.Id = "FormResponseCard";

            var columns = new List<AdaptiveColumn>
            {
                new AdaptiveColumn
                {
                    VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                    Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = trackerResponse,
                                Size = AdaptiveTextSize.Small,
                                Weight = AdaptiveTextWeight.Bolder,
                                Color = AdaptiveTextColor.Accent,
                                Wrap = true
                            }
                        }
                }
            };

            return card;
        }

        public static AdaptiveCard GetUserInputIncidentCard(string botId = null)
        {
            var card = new AdaptiveCard("1.0");

            var columns = new List<AdaptiveColumn>
            {
                new AdaptiveColumn
                {
                    VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                    Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = "Please Click Create Form To Create New Incident",
                                Size = AdaptiveTextSize.Small,
                                Weight = AdaptiveTextWeight.Bolder,
                                Color = AdaptiveTextColor.Accent,
                                Wrap = true
                            }
                        },
                }
            };

            var columnSet = new AdaptiveColumnSet
            {
                Columns = columns,
                Separator = true
            };

            var list = new List<AdaptiveElement>
            {
                columnSet
            };

            card.Body.AddRange(list);
            card?.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "Create Form",
                Data = new AdaptiveCardValue<TaskModuleMetadata>()
                {
                    Data = new TaskModuleMetadata()
                    {
                        SkillId = botId,
                        TaskModuleFlowType = TeamsFlowType.CreateSample_Form.ToString(),
                    }
                }
            });

            return card;
        }
    }
}
