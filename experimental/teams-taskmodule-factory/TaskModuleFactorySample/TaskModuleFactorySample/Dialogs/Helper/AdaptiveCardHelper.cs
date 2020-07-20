using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using Newtonsoft.Json;
using TaskModuleFactorySample.Extensions.Teams;
using TaskModuleFactorySample.Extensions.Teams.TaskModule;
using TaskModuleFactorySample.Models;

namespace TaskModuleFactorySample.Dialogs.Helper
{
    public class AdaptiveCardHelper
    {
        // TODO: Replace with Cards.Lg
        public static AdaptiveCard GetCardFromJson(string jsonFile)
        {
            string jsonCard = GetJson(jsonFile);

            return JsonConvert.DeserializeObject<AdaptiveCard>(jsonCard);
        }

        private static string GetJson(string jsonFile)
        {
            var dir = Path.GetDirectoryName(typeof(AdaptiveCardHelper).Assembly.Location);
            var filePath = Path.Combine(dir, $"{jsonFile}");
            return File.ReadAllText(filePath);
        }
    }
}
