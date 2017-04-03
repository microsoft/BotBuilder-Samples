namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    public class ActionExecutionContext
    {
        public ActionExecutionContext(string intent, ILuisAction action)
        {
            this.Intent = intent;
            this.Action = action;
        }

        public ILuisAction Action { get; private set; }

        public string Intent { get; private set; }

        public bool ChangeRootSignaling { get; set; }
    }
}
