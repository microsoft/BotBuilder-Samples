namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LuisActionBindingParamAttribute : Attribute
    {
        public string CustomType { get; set; }

        public string BuiltinType { get; set; }

        public int Order { get; set; }
    }
}
