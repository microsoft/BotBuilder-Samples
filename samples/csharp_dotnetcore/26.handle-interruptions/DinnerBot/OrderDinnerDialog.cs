using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace DinnerBot
{
    public class OrderDinnerDialogs : DialogSet
    {
        /// <summary>The ID of the top-level dialog.</summary>
        public const string MainDialogId = "mainMenu";

        /// <summary>The ID of the choice prompt.</summary>
        public const string ChoicePromptId = "choicePrompt";

        /// <summary>The ID of the order card value, tracked inside the dialog.</summary>
        public const string OrderCartId = "orderCart";

        public OrderDinnerDialogs(IStatePropertyAccessor<DialogState> dialogStateAccessor)
    : base(dialogStateAccessor)
        {
            // Add a choice prompt for the dialog.
            Add(new ChoicePrompt(ChoicePromptId));

            // Define and add the main waterfall dialog.
            WaterfallStep[] steps = new WaterfallStep[]
            {
        PromptUserAsync,
        ProcessInputAsync,
            };

            Add(new WaterfallDialog(MainDialogId, steps));
        }

        /// <summary>
        /// Defines the first step of the main dialog, which is to ask for input from the user.
        /// </summary>
        /// <param name="stepContext">The current waterfall step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task to perform.</returns>
        private async Task<DialogTurnResult> PromptUserAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Initialize order, continuing any order that was passed in.
            Order order = (stepContext.Options is Order oldCart && oldCart != null)
                ? new Order
                {
                    Items = new List<DinnerItem>(oldCart.Items),
                    Total = oldCart.Total,
                    ReadyToProcess = oldCart.ReadyToProcess,
                    OrderProcessed = oldCart.OrderProcessed,
                }
                : new Order();

            // Set the order cart in dialog state.
            stepContext.Values[OrderCartId] = order;

            // Prompt the user.
            return await stepContext.PromptAsync(
                "choicePrompt",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What would you like for dinner?"),
                    RetryPrompt = MessageFactory.Text(
                        "I'm sorry, I didn't understand that. What would you like for dinner?"),
                    Choices = ChoiceFactory.ToChoices(DinnerMenu.Choices),
                },
                cancellationToken);
        }

        /// <summary>
        /// Defines the second step of the main dialog, which is to process the user's input, and
        /// repeat or exit as appropriate.
        /// </summary>
        /// <param name="stepContext">The current waterfall step context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task to perform.</returns>
        private async Task<DialogTurnResult> ProcessInputAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the order cart from dialog state.
            Order order = stepContext.Values[OrderCartId] as Order;

            // Get the user's choice from the previous prompt.
            string response = (stepContext.Result as FoundChoice).Value;

            if (response.Equals("process order", StringComparison.InvariantCultureIgnoreCase))
            {
                order.ReadyToProcess = true;

                await stepContext.Context.SendActivityAsync(
                    "Your order is on it's way!",
                    cancellationToken: cancellationToken);

                // In production, you may want to store something more helpful.
                // "Process" the order and exit.
                order.OrderProcessed = true;
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else if (response.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                // Cancel the order.
                await stepContext.Context.SendActivityAsync(
                    "Your order has been canceled",
                    cancellationToken: cancellationToken);

                // Exit without processing the order.
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else if (response.Equals("more info", StringComparison.InvariantCultureIgnoreCase))
            {
                // Send more information about the options.
                string message = "More info: <br/>" +
                    "Potato Salad: contains 330 calories per serving. Cost: 5.99 <br/>"
                    + "Tuna Sandwich: contains 700 calories per serving. Cost: 6.89 <br/>"
                    + "Clam Chowder: contains 650 calories per serving. Cost: 4.50";
                await stepContext.Context.SendActivityAsync(
                    message,
                    cancellationToken: cancellationToken);

                // Continue the ordering process, passing in the current order cart.
                return await stepContext.ReplaceDialogAsync(MainDialogId, order, cancellationToken);
            }
            else if (response.Equals("help", StringComparison.InvariantCultureIgnoreCase))
            {
                // Provide help information.
                string message = "To make an order, add as many items to your cart as " +
                    "you like. Choose the `Process order` to check out. " +
                    "Choose `Cancel` to cancel your order and exit.";
                await stepContext.Context.SendActivityAsync(
                    message,
                    cancellationToken: cancellationToken);

                // Continue the ordering process, passing in the current order cart.
                return await stepContext.ReplaceDialogAsync(MainDialogId, order, cancellationToken);
            }

            // We've checked for expected interruptions. Check for a valid item choice.
            if (!DinnerMenu.MenuItems.ContainsKey(response))
            {
                await stepContext.Context.SendActivityAsync("Sorry, that is not a valid item. " +
                    "Please pick one from the menu.");

                // Continue the ordering process, passing in the current order cart.
                return await stepContext.ReplaceDialogAsync(MainDialogId, order, cancellationToken);
            }
            else
            {
                // Add the item to cart.
                DinnerItem item = DinnerMenu.MenuItems[response];

                order.Items.Add(item);
                order.Total += item.Price;

                // Acknowledge the input.
                await stepContext.Context.SendActivityAsync(
                    $"Added `{response}` to your order; your total is ${order.Total:0.00}.",
                    cancellationToken: cancellationToken);

                // Continue the ordering process, passing in the current order cart.
                return await stepContext.ReplaceDialogAsync(MainDialogId, order, cancellationToken);
            }
        }
        /// <summary>
        /// Contains information about an item on the menu.
        /// </summary>
        public class DinnerItem
        {
            public string Description { get; set; }

            public double Price { get; set; }
        }

        /// <summary>
        /// Describes the dinner menu, including the items on the menu and options for
        /// interrupting the ordering process.
        /// </summary>
        public class DinnerMenu
        {
            /// <summary>Gets the items on the menu.</summary>
            public static Dictionary<string, DinnerItem> MenuItems { get; } = new Dictionary<string, DinnerItem>
            {
                ["Potato salad"] = new DinnerItem { Description = "Potato Salad", Price = 5.99 },
                ["Tuna sandwich"] = new DinnerItem { Description = "Tuna Sandwich", Price = 6.89 },
                ["Clam chowder"] = new DinnerItem { Description = "Clam Chowder", Price = 4.50 },
            };

            /// <summary>Gets all the "interruptions" the bot knows how to process.</summary>
            public static List<string> Interrupts { get; } = new List<string>
    {
        "More info", "Process order", "Help", "Cancel",
    };

            /// <summary>Gets all of the valid inputs a user can make.</summary>
            public static List<string> Choices { get; }
                = MenuItems.Select(c => c.Key).Concat(Interrupts).ToList();
        }

        /// <summary>Helper class for storing the order.</summary>
        public class Order
        {
            public double Total { get; set; } = 0.0;

            public List<DinnerItem> Items { get; set; } = new List<DinnerItem>();

            public bool ReadyToProcess { get; set; } = false;

            public bool OrderProcessed { get; set; } = false;
        }


    }
}
