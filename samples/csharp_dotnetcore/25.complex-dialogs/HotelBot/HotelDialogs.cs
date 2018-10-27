using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace HotelBot
{
    /// <summary>Contains the set of dialogs and prompts for the hotel bot.</summary>
    public class HotelDialogs : DialogSet
    {
        /// <summary>The ID of the top-level dialog.</summary>
        public const string MainMenu = "mainMenu";

        public HotelDialogs(IStatePropertyAccessor<DialogState> dialogStateAccessor)
            : base(dialogStateAccessor)
        {
            // Add the prompts.
            Add(new ChoicePrompt(Inputs.Choice));
            Add(new NumberPrompt<int>(Inputs.Number));

            // Define the steps for and add the main welcome dialog.
            WaterfallStep[] welcomeDialogSteps = new WaterfallStep[]
            {
                MainDialogSteps.PresentMenuAsync,
                MainDialogSteps.ProcessInputAsync,
                MainDialogSteps.RepeatMenuAsync,
            };

            Add(new WaterfallDialog(MainMenu, welcomeDialogSteps));

            // Define the steps for and add the order-dinner dialog.
            WaterfallStep[] orderDinnerDialogSteps = new WaterfallStep[]
            {
                OrderDinnerSteps.StartFoodSelectionAsync,
                OrderDinnerSteps.GetRoomNumberAsync,
                OrderDinnerSteps.ProcessOrderAsync,
            };

            Add(new WaterfallDialog(Dialogs.OrderDinner, orderDinnerDialogSteps));

            // Define the steps for and add the order-prompt dialog.
            WaterfallStep[] orderPromptDialogSteps = new WaterfallStep[]
            {
                OrderPromptSteps.PromptForItemAsync,
                OrderPromptSteps.ProcessInputAsync,
            };

            Add(new WaterfallDialog(Dialogs.OrderPrompt, orderPromptDialogSteps));

            // Define the steps for and add the reserve-table dialog.
            WaterfallStep[] reserveTableDialogSteps = new WaterfallStep[]
            {
                ReserveTableSteps.StubAsync,
            };

            Add(new WaterfallDialog(Dialogs.ReserveTable, reserveTableDialogSteps));
        }

        /// <summary>Contains the IDs for the other dialogs in the set.</summary>
        private static class Dialogs
        {
            public const string OrderDinner = "orderDinner";
            public const string OrderPrompt = "orderPrompt";
            public const string ReserveTable = "reserveTable";
        }

        /// <summary>Contains the IDs for the prompts used by the dialogs.</summary>
        private static class Inputs
        {
            public const string Choice = "choicePrompt";
            public const string Number = "numberPrompt";
        }

        /// <summary>Contains the keys used to manage dialog state.</summary>
        private static class Outputs
        {
            public const string OrderCart = "orderCart";
            public const string OrderTotal = "orderTotal";
            public const string RoomNumber = "roomNumber";
        }

        /// <summary>Describes an option for the top-level dialog.</summary>
        private class WelcomeChoice
        {
            /// <summary>Gets or sets the text to show the guest for this option.</summary>
            public string Description { get; set; }

            /// <summary>Gets or sets the ID of the associated dialog for this option.</summary>
            public string DialogName { get; set; }
        }

        /// <summary>Describes an option for the food-selection dialog.</summary>
        /// <remarks>We have two types of options. One represents meal items that the guest
        /// can add to their order. The other represents a request to process or cancel the
        /// order.</remarks>
        private class MenuChoice
        {
            /// <summary>The request text for cancelling the meal order.</summary>
            public const string Cancel = "Cancel order";

            /// <summary>The request text for processing the meal order.</summary>
            public const string Process = "Process order";

            /// <summary>Gets or sets the name of the meal item or the request.</summary>
            public string Name { get; set; }

            /// <summary>Gets or sets the price of the meal item; or NaN for a request.</summary>
            public double Price { get; set; }

            /// <summary>Gets the text to show the guest for this option.</summary>
            public string Description => double.IsNaN(Price) ? Name : $"{Name} - ${Price:0.00}";
        }

        /// <summary>Contains the lists used to present options to the guest.</summary>
        private static class Lists
        {
            /// <summary>Gets the options for the top-level dialog.</summary>
            public static List<WelcomeChoice> WelcomeOptions { get; } = new List<WelcomeChoice>
            {
                new WelcomeChoice { Description = "Order dinner", DialogName = Dialogs.OrderDinner },
                new WelcomeChoice { Description = "Reserve a table", DialogName = Dialogs.ReserveTable },
            };

            private static readonly List<string> _welcomeList = WelcomeOptions.Select(x => x.Description).ToList();

            /// <summary>Gets the choices to present in the choice prompt for the top-level dialog.</summary>
            public static IList<Choice> WelcomeChoices { get; } = ChoiceFactory.ToChoices(_welcomeList);

            /// <summary>Gets the reprompt action for the top-level dialog.</summary>
            public static Activity WelcomeReprompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_welcomeList, "Please choose an option");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }

            /// <summary>Gets the options for the food-selection dialog.</summary>
            public static List<MenuChoice> MenuOptions { get; } = new List<MenuChoice>
    {
        new MenuChoice { Name = "Potato Salad", Price = 5.99 },
        new MenuChoice { Name = "Tuna Sandwich", Price = 6.89 },
        new MenuChoice { Name = "Clam Chowder", Price = 4.50 },
        new MenuChoice { Name = MenuChoice.Process, Price = double.NaN },
        new MenuChoice { Name = MenuChoice.Cancel, Price = double.NaN },
    };

            private static readonly List<string> _menuList = MenuOptions.Select(x => x.Description).ToList();

            /// <summary>Gets the choices to present in the choice prompt for the food-selection dialog.</summary>
            public static IList<Choice> MenuChoices { get; } = ChoiceFactory.ToChoices(_menuList);

            /// <summary>Gets the reprompt action for the food-selection dialog.</summary>
            public static Activity MenuReprompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_menuList, "Please choose an option");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the order-dinner dialog.
        /// </summary>
        private static class MainDialogSteps
        {
            public static async Task<DialogTurnResult> PresentMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Greet the guest and ask them to choose an option.
                await stepContext.Context.SendActivityAsync(
                    "Welcome to Contoso Hotel and Resort.",
                    cancellationToken: cancellationToken);
                return await stepContext.PromptAsync(
                    Inputs.Choice,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("How may we serve you today?"),
                        RetryPrompt = Lists.WelcomeReprompt,
                        Choices = Lists.WelcomeChoices,
                    },
                    cancellationToken);
            }

            public static async Task<DialogTurnResult> ProcessInputAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Begin a child dialog associated with the chosen option.
                var choice = (FoundChoice)stepContext.Result;
                var dialogId = Lists.WelcomeOptions[choice.Index].DialogName;

                return await stepContext.BeginDialogAsync(dialogId, null, cancellationToken);
            }

            public static async Task<DialogTurnResult> RepeatMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Start this dialog over again.
                return await stepContext.ReplaceDialogAsync(MainMenu, null, cancellationToken);
            }
        }

        /// <summary>Contains the guest's dinner order.</summary>
        private class OrderCart : List<MenuChoice>
        {
            public OrderCart() : base() { }

            public OrderCart(OrderCart other) : base(other) { }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the order-dinner dialog.
        /// </summary>
        private static class OrderDinnerSteps
        {
            public static async Task<DialogTurnResult> StartFoodSelectionAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.Context.SendActivityAsync(
                    "Welcome to our Dinner order service.",
                    cancellationToken: cancellationToken);

                // Start the food selection dialog.
                return await stepContext.BeginDialogAsync(Dialogs.OrderPrompt, null, cancellationToken);
            }

            public static async Task<DialogTurnResult> GetRoomNumberAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                if (stepContext.Result != null && stepContext.Result is OrderCart cart)
                {
                    // If there are items in the order, record the order and ask for a room number.
                    stepContext.Values[Outputs.OrderCart] = cart;
                    return await stepContext.PromptAsync(
                        Inputs.Number,
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("What is your room number?"),
                            RetryPrompt = MessageFactory.Text("Please enter your room number."),
                        },
                        cancellationToken);
                }
                else
                {
                    // Otherwise, assume the order was cancelled by the guest and exit.
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }
            }

            public static async Task<DialogTurnResult> ProcessOrderAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Get and save the guest's answer.
                var roomNumber = (int)stepContext.Result;
                stepContext.Values[Outputs.RoomNumber] = roomNumber;

                // Process the dinner order using the collected order cart and room number.

                await stepContext.Context.SendActivityAsync(
                    $"Thank you. Your order will be delivered to room {roomNumber} within 45 minutes.",
                    cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the order-prompt dialog.
        /// </summary>
        private static class OrderPromptSteps
        {
            public static async Task<DialogTurnResult> PromptForItemAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // First time through, options will be null.
                var cart = (stepContext.Options is OrderCart oldCart && oldCart != null)
                    ? new OrderCart(oldCart) : new OrderCart();

                stepContext.Values[Outputs.OrderCart] = cart;
                stepContext.Values[Outputs.OrderTotal] = cart.Sum(item => item.Price);

                return await stepContext.PromptAsync(
                    Inputs.Choice,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What would you like?"),
                        RetryPrompt = Lists.MenuReprompt,
                        Choices = Lists.MenuChoices,
                    },
                    cancellationToken);
            }

            public static async Task<DialogTurnResult> ProcessInputAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Get the guest's choice.
                var choice = (FoundChoice)stepContext.Result;
                var menuOption = Lists.MenuOptions[choice.Index];

                // Get the current order from dialog state.
                var cart = (OrderCart)stepContext.Values[Outputs.OrderCart];

                if (menuOption.Name is MenuChoice.Process)
                {
                    if (cart.Count > 0)
                    {
                        // If there are any items in the order, then exit this dialog,
                        // and return the list of selected food items.
                        return await stepContext.EndDialogAsync(cart, cancellationToken);
                    }
                    else
                    {
                        // Otherwise, send an error message and restart from
                        // the beginning of this dialog.
                        await stepContext.Context.SendActivityAsync(
                            "Your cart is empty. Please add at least one item to the cart.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(Dialogs.OrderPrompt, null, cancellationToken);
                    }
                }
                else if (menuOption.Name is MenuChoice.Cancel)
                {
                    await stepContext.Context.SendActivityAsync(
                        "Your order has been cancelled.",
                        cancellationToken: cancellationToken);

                    // Exit this dialog, returning null.
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }
                else
                {
                    // Add the selected food item to the order and update the order total.
                    cart.Add(menuOption);
                    var total = (double)stepContext.Values[Outputs.OrderTotal] + menuOption.Price;
                    stepContext.Values[Outputs.OrderTotal] = total;

                    await stepContext.Context.SendActivityAsync(
                        $"Added {menuOption.Name} (${menuOption.Price:0.00}) to your order." +
                            Environment.NewLine + Environment.NewLine +
                            $"Your current total is ${total:0.00}.",
                        cancellationToken: cancellationToken);

                    // Present the order options again, passing in the current order state.
                    return await stepContext.ReplaceDialogAsync(Dialogs.OrderPrompt, cart);
                }
            }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the reserve-table dialog.
        /// </summary>
        private static class ReserveTableSteps
        {
            public static async Task<DialogTurnResult> StubAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.Context.SendActivityAsync(
                    "Your table has been reserved.",
                    cancellationToken: cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
