﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerManager.Models;
using Microsoft.AspNetCore.Mvc;
using SiteHoster.Common.Services;
using SiteHosterSite.Services;

namespace DockerManager.Controllers
{
    public class DockerController : Controller
    {
        private DiscoveryServiceClient client = null;
        private readonly string bindingAddress = "";

        public DockerController()
        {
            var dockerHostName = Environment.GetEnvironmentVariable("DOCKER_HOST_NAME");
            var discoveryServiceUrl = Environment.GetEnvironmentVariable("DISCOVERY_URL");
            bindingAddress = Environment.GetEnvironmentVariable("BINDING_ADDRESS");
            
            this.client = new DiscoveryServiceClient(discoveryServiceUrl);
            var dockerHostSite = this.client.GetWebsite(dockerHostName);
            dockerHostSite.Wait();
            var dockerHost = dockerHostSite.Result.DockerUrl.ToString();

            if(string.IsNullOrEmpty(dockerHost))
            {
                DockerService.Host = "";
            }
            else
            {
                DockerService.Host = "-H " + dockerHost; // "-H unix:///var/run/docker.sock";
            }
        }

        [HttpGet]
        [Route("api/docker/ping")]
        public IActionResult Ping()
        {
            return this.Ok("Pong ");
        }


        [HttpPost()]
        [Route("api/docker/container/run/{serviceName}")]
        public async Task<IActionResult> Run(string serviceName, [FromBody]RunOptions options)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();

            if(website.DockerUrl != null)
            {
                return this.BadRequest("This website is already a container.  Do a start instead.  Url: " +website.DockerUrl);
            }

            var result = await DockerService.RunDockerImage(website.Name, options.port, "./");
            if(result.Any(p => p.IsError && !String.IsNullOrEmpty(p.Message)))
            {
                return this.BadRequest("Cannot create container for running.");
            }

            var containerId = await DockerService.GetContainerId(website.Name);
            var ports = await DockerService.GetContainerPortsExposed(containerId);

            var containerPort = ports.Select(p => p.Split(" -> ")[1]).FirstOrDefault();

            if(containerPort == null)
                return this.BadRequest("Could not get port manually.");
             
            website.DockerUrl = new Uri($"{bindingAddress}:{containerPort}");
            await this.client.UpdateWebsite(website.Name, website);
            return this.Ok("Ok");
        }

        
        [HttpPost()]
        [Route("api/docker/container/build/{serviceName}")]
        public async Task<IActionResult> Build(string serviceName)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.BuildDockerImage(website.Name, website.Path);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        [HttpPost()]
        [Route("api/docker/container/start/{serviceName}")]
        public async Task<IActionResult> Start(string serviceName)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.StartDockerImage(website.Name, null, website.Path);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        [HttpPost()]
        [Route("api/docker/container/stop/{serviceName}")]
        public async Task<IActionResult> Stop(string serviceName)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.StopDockerImage(website.Name);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
        
        [HttpPost()]
        [Route("api/docker/container/remove/{serviceName}")]
        public async Task<IActionResult> Remove(string serviceName)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.RemoveDockerContainer(website.Name);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
        
        [HttpPost()]
        [Route("api/docker/container/command/{serviceName}")]
        public async Task<IActionResult> Remove(string serviceName, [FromBody]CommandOptions options)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.ExecuteCommand(website.Name, options.command);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
        
        [HttpPost()]
        [Route("api/docker/container/copy/{serviceName}")]
        public async Task<IActionResult> Copy(string serviceName, [FromBody] CopyOptions options)
        {
            var website = await this.client.GetWebsite(serviceName);
            if(website == null) 
                return this.NotFound();
                
            var result = await DockerService.CopyFileToContainer(website.Name, options.hostPath, options.containerPath);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
    }
}
