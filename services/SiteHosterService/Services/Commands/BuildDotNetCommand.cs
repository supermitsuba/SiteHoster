namespace SiteHosterSite.Services.Commands
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class BuildDotNetCommand : Command
    {
        private readonly DotNetService service;
        private readonly DiscoveryServiceClient client;

        public BuildDotNetCommand(RabbitMessage receiver, DotNetService service, DiscoveryServiceClient client) :
            base(receiver)
        {
            this.receiver = receiver;
            this.service = service;
            this.client = client;
        }

        /*
        Test Format 
        {
        "RabbitMessageType":1,
        "message":"{ \"WebsiteName\": \"discovery-service\"}"
        }
        */
        public override async Task Execute()
        {
            // Get the website that needs to be built
            // TODO: call discovery service for that.
            var dotnetMessage = JsonConvert.DeserializeObject<DotNetMessage>(receiver.Message);
            Website site = await client.GetWebsite(dotnetMessage.WebsiteName);

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