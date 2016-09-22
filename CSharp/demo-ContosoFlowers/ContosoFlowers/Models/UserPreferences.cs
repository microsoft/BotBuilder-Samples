namespace ContosoFlowers.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class UserPreferences
    {
        public string SenderEmail { get; set; }

        public string SenderPhoneNumber { get; set; }

        public Dictionary<string, string> BillingAddresses { get; set; }
    }
}