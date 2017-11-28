namespace NginxManager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SiteHoster.Common.Services;
    using SiteHosterSite.Services;

    public class NginxController : Controller
    {
        public const string conatinerNameOfNginx = "nginx";
        public const string portExposed = "8080:80";
        public const string nginxConfigTemplatePath = "./nginx.conf";
        public readonly DiscoveryServiceClient discoveryService;

        public NginxController() // DiscoveryServiceClient client)
        {
            this.discoveryService = new DiscoveryServiceClient("http://localhost:7000");
        }

        // TODO: change the CLI calls to not be so suspectable to injection attacks
        [HttpPost()]
        [Route("api/nginx/container/create")]
        public async Task<IActionResult> Create()
        {
            var result = await DockerService.RunDockerImage(conatinerNameOfNginx, portExposed, "./");
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        [HttpPost()]
        [Route("api/nginx/start")]
        public async Task<IActionResult> Start()
        {
            var result = await DockerService.StartDockerImage(conatinerNameOfNginx, portExposed, "./");
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
        
        [HttpPost()]
        [Route("api/nginx/reload")]
        public async Task<IActionResult> Reload()
        {
            var result = await NginxService.ReloadNginxService(conatinerNameOfNginx);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);         
        }
        
        [HttpPost()]
        [Route("api/nginx/stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await DockerService.StopDockerImage(conatinerNameOfNginx);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        /*
    1. First create a base:
    worker_processes 1;

    error_log nginx_error.log;
    events {
        worker_connections 1024;
    }

    http {
        server {
            listen 80;

            location /discovery-service/ {
                proxy_pass http://192.168.10.125:7000/;
            }
        }
    }

    2.  Do a search for your application name: "\t\tlocation /{name}/ {"
      a. If it exists, replace the next line with the new address
    3. If it doesnt exist, add in the following:
      location /{name}/ {
          proxy_pass {docker url};
      }
    4. upload the change and restart nginx.
    5.  Is there a way to not restart nginx?  "service nginx reload"
     */
        [HttpPost()]
        [Route("api/nginx/config/{serviceName}")]
        public async Task<IActionResult> AddNewConfig(string serviceName)
        {
            var linesToWrite = new StringBuilder();
            var allLines = await System.IO.File.ReadAllLinesAsync(nginxConfigTemplatePath);
            var service = await this.discoveryService.GetWebsite(serviceName);

            for(var i = 0; i < allLines.Count(); i++)
            {
                if(i == 11)
                {
                    BuildNewApplication(linesToWrite, service.Name, service.DockerUrl.ToString());
                }
                else
                {
                    linesToWrite.AppendLine(allLines[i]);
                }
            }

            await System.IO.File.WriteAllTextAsync(nginxConfigTemplatePath, linesToWrite.ToString());
            
            var result = await NginxService.WriteConfig(conatinerNameOfNginx, nginxConfigTemplatePath);

            return this.Ok(result);
        }

        private void BuildNewApplication(StringBuilder stringBuilder, string name, string url)
        {
            stringBuilder.AppendLine($"");
            stringBuilder.AppendLine($"      location /{name}/ {{");
            stringBuilder.AppendLine($"        proxy_pass {url};");
            stringBuilder.AppendLine($"      }}");
            stringBuilder.AppendLine($"");
        }

    }
}
