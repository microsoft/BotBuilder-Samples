using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using V3StateBot.Models;

namespace V3StateBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        // Names of properties stored in CosmosDb UserData
        const string AskedNameProperty = "AskedName";
        const string GreetingStateProperty = "V3TestGreeting";
        const string TestDataClassProperty = "V3TestDataClass";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // An example of a string value saved directly to UserData
            context.UserData.SetValue("test", "test");

            // If we have not asked the user their name, ask it now
            var askedName = context.UserData.GetValueOrDefault<bool>(AskedNameProperty);
            if (!askedName)
            {
                await context.PostAsync($"v3: Hi.  What is your name?");
                // We've asked the user their name, so persist the information in UserData
                context.UserData.SetValue(AskedNameProperty, true);
            }
            else
            {
                // Check if we have saved GreetingState.  If we have not, assume the text is the user's Name in 
                // answer to the query above.
                var greetingState = context.UserData.GetValueOrDefault<GreetingState>(GreetingStateProperty);
                if (greetingState == null)
                {
                    greetingState = new GreetingState() { Name = activity.Text };
                    context.UserData.SetValue(GreetingStateProperty, greetingState);

                    await context.PostAsync($"v3: Hi {greetingState.Name}.  What city do you live in?");
                }
                else if (string.IsNullOrEmpty(greetingState.City))
                {
                    // Since greetingState.City is empty, assume the text is the user's City in response
                    // to the query above
                    greetingState.City = activity.Text;
                    context.UserData.SetValue(GreetingStateProperty, greetingState);

                    await context.PostAsync($"v3: Ok {greetingState.Name}.  I've got your City of residence as {greetingState.City}  Auto saving TestDataClass.");

                    // Save some random data
                    var classToSave = new TestDataClass() { TestIntField = 9,
                                                            TestStringField = Guid.NewGuid().ToString(),
                                                            TestTuple = new Tuple<string, string>("item 1", "item 2") };
                    context.UserData.SetValue(TestDataClassProperty, classToSave);

                }
                else
                {
                    int length = (activity.Text ?? string.Empty).Length;
                    // Return our reply to the user
                    await context.PostAsync($"v3: Hi {greetingState.Name} from {greetingState.City}.  You sent {activity.Text} which was {length} characters");

                    var testDataClass = context.UserData.GetValue<TestDataClass>(TestDataClassProperty);
                    await context.PostAsync($"v3: askedName: {askedName} testDataClass.TestIntField: {testDataClass.TestIntField} testDataClass.TestStringField: {testDataClass.TestStringField} testDataClass.TestTuple.Item1: {testDataClass.TestTuple.Item1} testDataClass.TestTuple.Item2: {testDataClass.TestTuple.Item2}");                    
                }
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}