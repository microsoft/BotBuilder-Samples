using System;

namespace Search.Dialogs.UserInteraction
{
    [Serializable]
    public class Button
    {
        public Button(string label, string message = null)
        {
            Label = label;
            Message = message ?? label;
        }

        public string Label { get; }
        public string Message { get; }
    }
}