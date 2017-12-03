namespace SiteHoster.Common.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Extensions;
    using System.Text;

    public class DockerServiceClient
    {
        private string baseAddress;
        public DockerServiceClient(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public async Task<string> RunContainer(string name, string portExposed)
        {
            using(var client = new HttpClient())
            {
                var myObject = new { port = portExposed };
                var json =JsonConvert.SerializeObject(myObject);
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/run/{name}", new StringContent(json, Encoding.ASCII, "application/json"));
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> StartContainer(string name)
        {
            using(var client = new HttpClient())
            {
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/start/{name}", new StringContent(""));
                return await result.Content.ReadAsStringAsync();
            }
        }
        
        public async Task<string> BuildContainer(string name)
        {
            using(var client = new HttpClient())
            {
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/build/{name}", new StringContent(""));
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> StopContainer(string name)
        {
            using(var client = new HttpClient())
            {
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/stop/{name}", new StringContent(""));
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> CopyFileToContainer(string name, string hostPath, string containerPath)
        {
            using(var client = new HttpClient())
            {
                var myObject = new { hostPath = hostPath, containerPath = containerPath };
                var json =JsonConvert.SerializeObject(myObject);
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/copy/{name}", new StringContent(json));
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> ExecuteCommandInContainer(string name, string command)
        {
            using(var client = new HttpClient())
            {
                var myObject = new { command = command };
                var json =JsonConvert.SerializeObject(myObject);                
                var result = await client.PostAsync($"{baseAddress}/api/docker/container/command/{name}", new StringContent(json));
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}