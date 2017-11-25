namespace SiteHosterSite.Services.Commands
{
    using System.Diagnostics;
    using System.Net.Http;
    using Newtonsoft.Json;
    using SiteHoster.Common.Models;

    public class BuildDockerCommand : Command
    {
        private readonly DockerService service;

        public BuildDockerCommand(RabbitMessage receiver, DockerService service) :
            base(receiver)
        {
            this.receiver = receiver;
            this.service = service;
        }

        /*
        Test Format 
        {
        "RabbitMessageType":0,
        "message":"{ \"WebsiteName\": \"discovery-service\"}"
        }
        */
        public override async void Execute()
        {
            // Get the website that needs to be built
            // TODO: call discovery service for that.
            System.Console.WriteLine("[DEBUG] - Execute Docker Command."); 
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