namespace WebsiteManager.Controllers
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Net;
    using SiteHoster.Common.Models;
    using WebsiteManager.Models;

    [Route("api/[controller]")]
    public class WebsitesController : Controller
    {
        private readonly ConfigurationModel configuration;

        public WebsitesController(IOptions<ConfigurationModel> config)
        {
            this.configuration = config.Value;
            InitializeDirectory(this.configuration.saveLocation);
        }

        private static void InitializeDirectory(string path)
        {
            // Probably should be replaced by singleton with a lock pattern
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Website> Get()
        {
            var websites = GetAllRecords(this.configuration.saveLocation);
            return websites;
        }

        private IEnumerable<Website> GetAllRecords(string path)
        {
            var results = new List<Website>();
            
            // probably should cache the results.
            foreach(var jsonFile in Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly))
            {
                var text = System.IO.File.ReadAllText(jsonFile);
                results.Add(JsonConvert.DeserializeObject<Website>(text));
            }

            return results;
        }

        // GET api/values/5
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var result = GetAllRecords(this.configuration.saveLocation).Where(p => p.Name == name).FirstOrDefault(); 
            if(result == null){
                return this.StatusCode((int)HttpStatusCode.NotFound);
            }

            return this.StatusCode((int)HttpStatusCode.OK, result);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Website value)
        {
            if(!NameExist(this.configuration.saveLocation, value.Name)) {
                SaveWebsite(this.configuration.saveLocation, value);
                return this.StatusCode((int)HttpStatusCode.Created, value);
            }
            else {
                return this.StatusCode((int)HttpStatusCode.Conflict, "Name already exists.");
            }
        }

        private static void SaveWebsite(string path, Website value)
        {
            var fullPath = System.IO.Path.Combine(path, value.Name+".json");
            System.IO.File.WriteAllText(fullPath, JsonConvert.SerializeObject(value));
        }

        private static bool NameExist(string path, string name)
        {
            return Directory.EnumerateFiles(path, $"{name}.json", SearchOption.TopDirectoryOnly).Any();
        }

        // PUT api/values/5
        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody]Website value)
        {
            if(name == value.Name){
                SaveWebsite(this.configuration.saveLocation, value);
                return this.StatusCode((int)HttpStatusCode.OK, value);
            }
            else{
                return this.StatusCode((int)HttpStatusCode.Conflict, "The name in the url and the name in the website do not match.");
            }
        }

        // DELETE api/values/5
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            if(NameExist(this.configuration.saveLocation, name)) {
                DeleteWebsite(this.configuration.saveLocation, name);
                return this.StatusCode((int)HttpStatusCode.OK);
            }
            else {
                return this.StatusCode((int)HttpStatusCode.NotFound);
            }
        }

        private void DeleteWebsite(string saveLocation, string name)
        {
            var fullPath = System.IO.Path.Combine(saveLocation, name+".json");
            System.IO.File.Delete(fullPath);
        }
    }
}
