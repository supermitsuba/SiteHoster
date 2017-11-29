namespace SiteHosterSite.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class NginxService
    {
        public static string Host {get;set;}
        public static Task<List<ConsoleMessage>> ReloadNginxService(string nameOfNginxService)
        {
            var command = "docker";
            var args = $"{Host} exec -d {nameOfNginxService} /usr/sbin/nginx -s reload";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> WriteConfig(string nginxName, string configPath)
        {
            var command = "docker";
            var args = $"{Host} cp {System.IO.Path.GetFullPath(configPath)} {nginxName}:/etc/nginx/nginx.conf";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }
    }
}