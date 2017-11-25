namespace SiteHosterSite.Services.Commands
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class BuildDockerCommand : Command
    {
        private readonly DockerService service;
        private readonly DiscoveryServiceClient client;

        public BuildDockerCommand(RabbitMessage receiver, DockerService service, DiscoveryServiceClient client) :
            base(receiver)
        {
            this.receiver = receiver;
            this.service = service;
            this.client = client;
        }

        /*
        Test Format 
        {
        "RabbitMessageType":0,
        "message":"{ \"WebsiteName\": \"discovery-service\"}"
        }
        */
        public override async Task Execute()
        {
            // Get the website that needs to be built
            // TODO: call discovery service for that.
            System.Console.WriteLine("[DEBUG] - Execute Docker Command."); 
            var dotnetMessage = JsonConvert.DeserializeObject<DotNetMessage>(receiver.Message);
            Website site = await client.GetWebsite(dotnetMessage.WebsiteName);

            if(site != null)
            {
                var nameOfApplication = site.Name;
                var directoryOfDockerfile = site.Path;

                this.service.BuildDockerImage(nameOfApplication, directoryOfDockerfile);

                // throw new command to run docker image
                this.service.RunDockerImage(nameOfApplication, directoryOfDockerfile);
            }
            else
            {
                System.Console.WriteLine("[WARN] - Could not parse website.");
            }
        }
    }
}