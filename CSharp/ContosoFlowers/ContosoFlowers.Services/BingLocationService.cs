namespace ContosoFlowers.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ContosoFlowers.Services.BingMaps;

    public class BingLocationService : ILocationService
    {
        private readonly BingMapsService service;

        public BingLocationService(string bingMapsKey)
        {
            this.service = new BingMapsService(bingMapsKey);
        }

        public async Task<IEnumerable<string>> ParseAddressAsync(string address)
        {
            var results = await this.service.GeoCode(address);
            return results.Select(o => o.FormattedAddress);
        }
    }
}