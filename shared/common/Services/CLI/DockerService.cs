namespace SiteHosterSite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class DockerService 
    {
        public static Task<List<ConsoleMessage>> BuildDockerImage(string nameOfApplication, string directoryOfDockerfile)
        {
            var process = new ProcessExecutor();
            var command = "docker";
            var args = $"build -t supermitsuba/{nameOfApplication}:1 . --no-cache";
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
            var args = $"run --name {nameOfApplication} -d -p {port} supermitsuba/{nameOfApplication}:1";
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
            var args = $"start {nameOfApplication}";
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
            var args = $"stop {nameOfApplication}";
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