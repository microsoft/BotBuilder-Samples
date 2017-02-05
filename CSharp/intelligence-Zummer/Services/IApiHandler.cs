using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zummer.Services
{
    public interface IApiHandler
    {
        Task<T> GetJsonAsync<T>(string url, IDictionary<string, string> requestParameters = null, IDictionary<string, string> headers = null) where T : class;

        Task<string> GetStringAsync(string url, IDictionary<string, string> requestParameters = null, IDictionary<string, string> headers = null);
    }
}
