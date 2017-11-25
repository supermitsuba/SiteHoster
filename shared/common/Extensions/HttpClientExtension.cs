namespace SiteHoster.Common.Extensions
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;

    public static class HttpClientExtension
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string url)
        {
            var result = default(T);
            var httpResult = await client.GetAsync(url);
            if(httpResult.IsSuccessStatusCode)
            {
                var contentResult = await httpResult.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(contentResult);
            }
            else
            {
                return default(T);
            }

            return result;
        }
    }
}