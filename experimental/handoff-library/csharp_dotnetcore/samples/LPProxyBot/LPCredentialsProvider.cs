using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LivePersonConnector;
using Microsoft.Extensions.Configuration;

namespace LPProxyBot
{
    public class LPCredentialsProvider : ICredentialsProvider
    {
        public LPCredentialsProvider(IConfiguration configuration)
        {
            LpAccount = configuration["LivePersonAccount"];
#if RELEASE
            LpAppId = configuration["LivePersonClientId_Release"];
            LpAppSecret = configuration["LivePersonClientSecret_Release"];
#else
            LpAppId = configuration["LivePersonClientId"];
            LpAppSecret = configuration["LivePersonClientSecret"];
#endif
            MsAppId = configuration["MicrosoftAppId"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(MsAppId))
            {
                MsAppId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        public string LpAccount { get; }

        public string LpAppId { get; }

        public string LpAppSecret { get; }

        public string MsAppId { get; }
    }
}
