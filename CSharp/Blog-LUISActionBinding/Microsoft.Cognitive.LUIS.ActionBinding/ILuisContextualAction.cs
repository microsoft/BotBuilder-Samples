namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    public interface ILuisContextualAction<T> : ILuisAction where T : ILuisAction
    {
        T Context { get; set; }
    }
}
