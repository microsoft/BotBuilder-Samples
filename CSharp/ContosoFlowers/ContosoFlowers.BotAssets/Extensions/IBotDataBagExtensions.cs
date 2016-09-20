namespace ContosoFlowers.BotAssets.Extensions
{
    using System;
    using Microsoft.Bot.Builder.Dialogs;

    public static class IBotDataBagExtensions
    {
        /// <summary>
        /// Retrieves, initializes and manipulates an IBotDataBag value.
        /// </summary>
        /// <typeparam name="T">The generic type of the value to be retrieved or initialized. Must allow new().</typeparam>
        /// <param name="botDataBag"> UserData, ConversationData or PrivateConversationData.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="updateAction">A lamba Action<T> to manipulate the retrieve or new value to save.</param>
        public static void UpdateValue<T>(this IBotDataBag botDataBag, string key, Action<T> updateAction) where T : new()
        {
            T value = default(T);
            if (!botDataBag.TryGetValue(key, out value))
            {
                value = new T();
            }

            updateAction(value);
            botDataBag.SetValue(key, value);
        }
    }
}