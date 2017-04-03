namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Ionic.Zip;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using Models;

    [Serializable]
    [LuisActionBinding("FindAirportByCode", FriendlyName = "Find Airport by Code")]
    public class FindAirportByCodeAction : BaseLuisAction
    {
        [Required(ErrorMessage = "Please provide airport CODE")]
        [Location(ErrorMessage = "Please provide a valid CODE")]
        public string Code { get; set; }

        public override Task<object> FulfillAsync()
        {
            using (var stream = UnZipCatalog())
            {
                var result = default(AirportInfo);
                var msg = "Airport CODE not found, please try another query!";

                var geoCatalog = XElement.Load(stream);
                var airport = geoCatalog.XPathSelectElement($"//Airport[@Id=\"{this.Code.ToUpperInvariant()}\"]");
                if (airport != null)
                {
                    var city = airport.Parent.Parent;
                    var country = city.Parent.Parent;

                    result = new AirportInfo
                    {
                        City = city.Attribute("Name").Value,
                        Code = this.Code.ToUpperInvariant(),
                        Country = country.Attribute("Name").Value,
                        Location = airport.Attribute("Location").Value,
                        Name = airport.Attribute("Name").Value
                    };
                    msg = $"{this.Code.ToUpperInvariant()} corresponds to \"{airport.Attribute("Name").Value}\" which is located in {city.Attribute("Name").Value}, {country.Attribute("Name").Value} [{airport.Attribute("Location").Value}]";
                }

                return Task.FromResult((object)result);
            }
        }

        private static MemoryStream UnZipCatalog()
        {
            // embedded resource got from http://partners.api.skyscanner.net//apiservices//geo//v1.0?apikey=prtl6749387986743898559646983194
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "LuisActions.Samples.Assets.airportCatalog.zip";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                MemoryStream data = new MemoryStream();
                using (ZipFile zip = ZipFile.Read(stream))
                {
                    zip["airportCatalog.xml"].Extract(data);
                }

                data.Seek(0, SeekOrigin.Begin);
                return data;
            }
        }
    }
}