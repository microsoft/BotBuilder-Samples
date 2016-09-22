namespace ContosoFlowers.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationService
    {
        Task<IEnumerable<string>> ParseAddressAsync(string text);
    }
}
