namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    // TODO: add missing types
    public class BuiltInEncyclopediaTypes
    {
        public const string Person = "builtin.encyclopedia.people.person";

        public const string Organization = "builtin.encyclopedia.organization.organization";

        public const string Event = "builtin.encyclopedia.time.event";

        public string PersonType
        {
            get { return Person; }
        }

        public string OrganizationType
        {
            get { return Organization; }
        }

        public string EventType
        {
            get { return Event; }
        }
    }
}
