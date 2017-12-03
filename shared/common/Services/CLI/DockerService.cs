namespace SiteHosterSite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class DockerService 
    {
        public static string Host {get;set;}

        public static Task<List<ConsoleMessage>> BuildDockerImage(string nameOfApplication, string directoryOfDockerfile)
        {
            var process = new ProcessExecutor();
            var command = "docker";
            var args = $"{Host} build -t supermitsuba/{nameOfApplication}:1 . --no-cache";
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, directoryOfDockerfile, message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> RunDockerImage(string nameOfApplication, string port, string directoryOfDockerfile)
        {
            var process = new ProcessExecutor();
            var command = "docker";
            var args = "";
            if(string.IsNullOrEmpty(port))
            {
                args = $"{Host} run --name {nameOfApplication} -d -P supermitsuba/{nameOfApplication}:1";
            }
            else 
            {
                args = $"{Host} run --name {nameOfApplication} -d -p {port} supermitsuba/{nameOfApplication}:1";
            }

            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, directoryOfDockerfile, message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> StartDockerImage(string nameOfApplication, string port, string directoryOfDockerfile)
        {
            var process = new ProcessExecutor();
            var command = "docker";
            var args = $"{Host} start {nameOfApplication}";
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, directoryOfDockerfile, message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> StopDockerImage(string nameOfApplication)
        {
            var command = "docker";
            var args = $"{Host} stop {nameOfApplication}";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> RemoveDockerContainer(string nameOfApplication)
        {
            var command = "docker";
            var args = $"{Host} rm {nameOfApplication}";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> InspectDockerContainer(string containerId, string format)
        {
            var command = "docker";
            var args = Host + " inspect " +  format +" "+ containerId;
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public async static Task<IEnumerable<string>> GetContainerPortsExposed(string containerId)
        {
            var format = "--format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{$p}} -> {{(index $conf 0).HostPort}} {{end}}\"";
            var result2 = await DockerService.InspectDockerContainer(containerId, format);
            var portMapping = result2.Where(p => !p.IsError)
                                     .Select(p => p.Message.Replace("[DEBUG]:", "").Trim())
                                     .Where(p => !string.IsNullOrEmpty(p));
            return portMapping;
        }

        public async static Task<string> GetContainerId(string name)
        {
            var format = "--format=\"{{.Id}}\"";
            var result2 = await DockerService.InspectDockerContainer(name, format);
            var id = result2.Where(p => !p.IsError)
                            .Select(p => p.Message.Replace("[DEBUG]:", "").Trim())
                            .Where(p => !string.IsNullOrEmpty(p))
                            .FirstOrDefault();

            return id;
        }

        public static Task<List<ConsoleMessage>> ExecuteScript(string pathToScript)
        {
            var command = $"{pathToScript}";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, "", "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }
        public static Task<List<ConsoleMessage>> ExecuteCommand(string nameOfNginxService, string command)
        {
            var dockerCommand = "docker";
            var args = $"{Host} exec -d {nameOfNginxService} {command}";
            var process = new ProcessExecutor();
            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(dockerCommand, args, "./", message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> CopyFileToContainer(string nginxName, string hostPath, string containerPath)
        {
            var command = "docker";
            var args = $"{Host} cp {hostPath} {nginxName}:{containerPath}";
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