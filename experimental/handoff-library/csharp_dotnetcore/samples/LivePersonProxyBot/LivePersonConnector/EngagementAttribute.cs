using System;
using System.Collections.Generic;
using System.Text;

namespace LivePersonConnector
{
    public class EngagementAttribute
    {
        public string Type { get; set; }
        public string SocialId { get; set; }
        public string CustomerType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        public Models.Lp_Sdes ToLivePersonEngagementAttribute()
        {
            switch(Type)
            {
                case "ctmrinfo" : return new Models.Lp_Sdes { type = Type, info = new Models.Info { socialId = SocialId, ctype = CustomerType } };
                case "personal" : return new Models.Lp_Sdes { type = Type, personal = new Models.Personal { firstname = FirstName, lastname = LastName, gender = Gender } };
                default:
                    throw new NotSupportedException("Wrong engagement type");
            }
        }
    }
}
