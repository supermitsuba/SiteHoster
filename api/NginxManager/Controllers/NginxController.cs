namespace NginxManager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SiteHoster.Common.Services;
    using SiteHosterSite.Services;

    public class NginxController : Controller
    {
        public const string conatinerNameOfNginx = "nginx";
        public readonly string portExposed;
        public const string nginxConfigTemplatePath = "./nginx.conf";
        public readonly DockerServiceClient dockerServiceClient;
        public readonly DiscoveryServiceClient discoveryServiceClient;
        public readonly string nginxLocation;
        public readonly string dockerLocation;

        public NginxController() // DiscoveryServiceClient client)
        {
            this.nginxLocation = Environment.GetEnvironmentVariable("NGINX_LOCATIONS");
            this.dockerLocation = Environment.GetEnvironmentVariable("DOCKER_LOCATIONS");
            InitializeDirectory(nginxLocation);
            var dockerHost = Environment.GetEnvironmentVariable("DOCKER_URL");
            var discoveryServiceUrl = Environment.GetEnvironmentVariable("DISCOVERY_URL");
            var dockerServiceUrl = Environment.GetEnvironmentVariable("DISCOVERY_URL");
            var port = Environment.GetEnvironmentVariable("NGINX_EXPOSED_PORT");

            if(string.IsNullOrEmpty(dockerHost))
            {
                DockerService.Host = "";
            }
            else
            {
                DockerService.Host = "-H " + dockerHost; // "-H unix:///var/run/docker.sock";
            }

            if(string.IsNullOrEmpty(port))
            {
                portExposed = "8080:80";
            }
            else
            {
                portExposed = port + ":80";
            }

            this.dockerServiceClient = new DockerServiceClient(dockerServiceUrl);
            this.discoveryServiceClient = new DiscoveryServiceClient(discoveryServiceUrl);
        }

        // TODO: change the CLI calls to not be so suspectable to injection attacks
        [HttpPost()]
        [Route("api/nginx/container/create")]
        public async Task<IActionResult> Create()
        {
            var result = await this.dockerServiceClient.RunContainer("nginx", portExposed+":80");
            return this.Ok(result);
        }

        [HttpPost()]
        [Route("api/nginx/start")]
        public async Task<IActionResult> Start()
        {
            var result = await this.dockerServiceClient.StartContainer("nginx");
            return this.Ok(result);
        }
        
        [HttpPost()]
        [Route("api/nginx/reload")]
        public async Task<IActionResult> Reload()
        {
            var result = await this.dockerServiceClient.ExecuteCommandInContainer("nginx", "/usr/sbin/nginx -s reload");
            return this.Ok(result);       
        }
        
        [HttpPost()]
        [Route("api/nginx/stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await this.dockerServiceClient.StopContainer("nginx");
            return this.Ok(result);
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
            var service = await this.discoveryServiceClient.GetWebsite(serviceName);

            await SaveNewApplication(service.Name, service.DockerUrl.ToString());

            for(var i = 0; i < allLines.Count(); i++)
            {
                if(i == 11)
                {
                    var config = await this.WriteNginxConfig();
                    linesToWrite.Append(config);
                }
                else
                {
                    linesToWrite.AppendLine(allLines[i]);
                }
            }

            var nginxConfigPath = System.IO.Path.Combine(this.dockerLocation, nginxConfigTemplatePath+".new");
            await System.IO.File.WriteAllTextAsync(nginxConfigPath, linesToWrite.ToString());
            
            var result = await this.dockerServiceClient.CopyFileToContainer("nginx", nginxConfigPath, "/etc/nginx/nginx.conf");

            return this.Ok(result);
        }

        private async Task SaveNewApplication(string name, string url)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"");
            stringBuilder.AppendLine($"      location /{name}/ {{");
            stringBuilder.AppendLine($"        proxy_pass {url};");
            stringBuilder.AppendLine($"      }}");
            stringBuilder.AppendLine($"");
            var path = System.IO.Path.Combine(this.nginxLocation, name+".json");
            await System.IO.File.WriteAllTextAsync(path, stringBuilder.ToString());
        }

        private async Task<string> WriteNginxConfig()
        {
            var stringBuilder = new StringBuilder();

            foreach(var jsonFile in Directory.EnumerateFiles(this.nginxLocation, "*.json", SearchOption.TopDirectoryOnly))
            {
                var text = await System.IO.File.ReadAllTextAsync(jsonFile);
                stringBuilder.Append(text);
            }

            return stringBuilder.ToString();
        }

        private static void InitializeDirectory(string path)
        {
            // Probably should be replaced by singleton with a lock pattern
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
