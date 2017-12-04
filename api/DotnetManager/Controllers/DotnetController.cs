using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteHoster.Common.Services;
using SiteHosterSite.Services;

namespace DotnetManager.Controllers
{
    public class DotnetController : Controller
    {
        private readonly DiscoveryServiceClient discoveryServiceClient;

        public DotnetController()
        {
            this.discoveryServiceClient = new DiscoveryServiceClient("http://192.168.10.125:8000");
        }

        // GET api/values
        [HttpPost]
        [Route("api/dotnet/{name}/create")]
        public async Task<IActionResult> Create(string name)
        {
            var getWebsite = await this.discoveryServiceClient.GetWebsite(name);

            if(name == null) 
                return this.NotFound();

            var result = await DotNetService.CreateDotnetProject(getWebsite.Path);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        // GET api/values
        [HttpPost]
        [Route("api/dotnet/{name}/build")]
        public  async Task<IActionResult>  Build(string name)
        {
            var getWebsite = await this.discoveryServiceClient.GetWebsite(name);

            if(name == null) 
                return this.NotFound();

            var result = await DotNetService.BuildDotnetBinary(getWebsite.Path);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }

        // GET api/values
        [HttpPost]
        [Route("api/dotnet/{name}/restore")]
        public  async Task<IActionResult>  restore(string name)
        {
            var getWebsite = await this.discoveryServiceClient.GetWebsite(name);

            if(name == null) 
                return this.NotFound();

            var result = await DotNetService.RestoreDotnetPackages(getWebsite.Path);
            var message = string.Join("\n", result.Select(p => (p.IsError ? "[ERROR]: " : "[Message]: ") + p.Message));
            return this.Ok(message);
        }
    }
}
