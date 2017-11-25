namespace SiteHosterSite.Services.Commands
{
    using System.Diagnostics;
    using System.Net.Http;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;

    public class BuildDotNetCommand : Command
    {
        private readonly DotNetService service;

        public BuildDotNetCommand(RabbitMessage receiver, DotNetService service) :
            base(receiver)
        {
            this.receiver = receiver;
            this.service = service;
        }

        /*
        Test Format 
        {
        "RabbitMessageType":1,
        "message":"{ \"WebsiteName\": \"discovery-service\"}"
        }
        */
        public override async void Execute()
        {
            // Get the website that needs to be built
            // TODO: call discovery service for that.
            var dotnetMessage = JsonConvert.DeserializeObject<DotNetMessage>(receiver.Message);
            Website site = null;
            using(var client = new HttpClient())
            {
                var httpResult = await client.GetAsync($"http://localhost:5000/api/websites/{dotnetMessage.WebsiteName}");
                if(httpResult.IsSuccessStatusCode)
                {
                    var contentResult = await httpResult.Content.ReadAsStringAsync();
                    site = JsonConvert.DeserializeObject<Website>(contentResult);
                }
                else
                {
                    System.Console.WriteLine("[WARN] - Could not retrieve service from discovery service.");
                    return;
                }
            }

            if(site != null)
            {
                System.Console.WriteLine("[DEBUG] - Execute Dotnet Command.");
                var pathOfCode = site.Path;

                this.service.RestoreDotnetPackages(pathOfCode);

                this.service.BuildDotnetBinary(pathOfCode);
            }
            else
            {
                System.Console.WriteLine("[WARN] - Could not parse website.");
            }
        }
    }
}