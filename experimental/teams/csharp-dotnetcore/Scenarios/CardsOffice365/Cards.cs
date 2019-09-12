// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Schema.Teams;

namespace Cards
{
    public static class Cards
    {
        public static O365ConnectorCard CreateSampleO365ConnectorCard()
        {
            var actionCard1 = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Multiple Choice",
                "card-1",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "list-1",
                        true,
                        "Pick multiple options",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Choice 1", "1"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice 2", "2"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice 3", "3")
                        },
                        "expanded",
                        true),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "list-2",
                        true,
                        "Pick multiple options",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Choice 4", "4"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice 5", "5"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice 6", "6")
                        },
                        "compact",
                        true),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "list-3",
                        false,
                        "Pick an option",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Choice a", "a"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice b", "b"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice c", "c")
                        },
                        "expanded",
                        false),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "list-4",
                        false,
                        "Pick an option",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Choice x", "x"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice y", "y"),
                            new O365ConnectorCardMultichoiceInputChoice("Choice z", "z")
                        },
                        "compact",
                        false)
    },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "card-1-btn-1",
                        @"{""list1"":""{{list-1.value}}"", ""list2"":""{{list-2.value}}"", ""list3"":""{{list-3.value}}"", ""list4"":""{{list-4.value}}""}")
                });

            var actionCard2 = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Text Input",
                "card-2",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-1",
                        false,
                        "multiline, no maxLength",
                        null,
                        true,
                        null),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-2",
                        false,
                        "single line, no maxLength",
                        null,
                        false,
                        null),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-3",
                        true,
                        "multiline, max len = 10, isRequired",
                        null,
                        true,
                        10),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-4",
                        true,
                        "single line, max len = 10, isRequired",
                        null,
                        false,
                        10)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "card-2-btn-1",
                        @"{""text1"":""{{text-1.value}}"", ""text2"":""{{text-2.value}}"", ""text3"":""{{text-3.value}}"", ""text4"":""{{text-4.value}}""}")
                });

            var actionCard3 = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Date Input",
                "card-3",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardDateInput(
                        O365ConnectorCardDateInput.Type,
                        "date-1",
                        true,
                        "date with time",
                        null,
                        true),
                    new O365ConnectorCardDateInput(
                        O365ConnectorCardDateInput.Type,
                        "date-2",
                        false,
                        "date only",
                        null,
                        false)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "card-3-btn-1",
                        @"{""date1"":""{{date-1.value}}"", ""date2"":""{{date-2.value}}""}")
                });

            var section = new O365ConnectorCardSection(
                "**section title**",
                "section text",
                "activity title",
                "activity subtitle",
                "activity text",
                "http://connectorsdemo.azurewebsites.net/images/MSC12_Oscar_002.jpg",
                "avatar",
                true,
                new List<O365ConnectorCardFact>
                {
                    new O365ConnectorCardFact("Fact name 1", "Fact value 1"),
                    new O365ConnectorCardFact("Fact name 2", "Fact value 2"),
                },
                new List<O365ConnectorCardImage>
                {
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/MicrosoftSurface_024_Cafe_OH-06315_VS_R1c.jpg",
                        Title = "image 1"
                    },
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/WIN12_Scene_01.jpg",
                        Title = "image 2"
                    },
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/WIN12_Anthony_02.jpg",
                        Title = "image 3"
                    }
                });

            O365ConnectorCard card = new O365ConnectorCard()
            {
                Summary = "O365 card summary",
                ThemeColor = "#E67A9E",
                Title = "card title",
                Text = "card text",
                Sections = new List<O365ConnectorCardSection> { section },
                PotentialAction = new List<O365ConnectorCardActionBase>
                    {
                        actionCard1,
                        actionCard2,
                        actionCard3,
                        new O365ConnectorCardViewAction(
                            O365ConnectorCardViewAction.Type,
                            "View Action",
                            null,
                            new List<string>
                            {
                                "http://microsoft.com"
                            }),
                        new O365ConnectorCardOpenUri(
                            O365ConnectorCardOpenUri.Type,
                            "Open Uri",
                            "open-uri",
                            new List<O365ConnectorCardOpenUriTarget>
                            {
                                new O365ConnectorCardOpenUriTarget
                                {
                                    Os = "default",
                                    Uri = "http://microsoft.com"
                                },
                                new O365ConnectorCardOpenUriTarget
                                {
                                    Os = "iOS",
                                    Uri = "http://microsoft.com"
                                },
                                new O365ConnectorCardOpenUriTarget
                                {
                                    Os = "android",
                                    Uri = "http://microsoft.com"
                                },
                                new O365ConnectorCardOpenUriTarget
                                {
                                    Os = "windows",
                                    Uri = "http://microsoft.com"
                                }
                            })
                    }
            };

            return card;
        }
    }
}
