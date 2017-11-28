namespace SiteHoster.Common.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Extensions;

    public class DiscoveryServiceClient
    {
        private string baseAddress;
        public DiscoveryServiceClient(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public async Task<Website> GetWebsite(string name)
        {
            Website site = null;
            using(var client = new HttpClient())
            {
                site = await client.GetAsync<Website>($"{baseAddress}/api/websites/{name}");
            }

            return site;
        }

        public async Task<Website> UpdateWebsite(string name, Website site)
        {
            using(var client = new HttpClient())
            {
                site = await client.PutAsync<Website>($"{baseAddress}/api/websites/{name}", site);
            }

            return site;
        }
    }
}