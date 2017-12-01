using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.CommandLine;

namespace DockerManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .AddEnvironmentVariables()
                            .Build();

            return WebHost.CreateDefaultBuilder(args)
                            .UseConfiguration(config)
                            .UseKestrel()
                            .UseUrls("http://0.0.0.0:5000")
                            .UseStartup<Startup>()
                            .Build();
        }
    }
}
