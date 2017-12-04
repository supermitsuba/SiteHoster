namespace SiteHosterSite.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using SiteHoster.Common.Models;
    using SiteHoster.Common.Services;

    public class DotNetService
    {
        public static Task<List<ConsoleMessage>> CreateDotnetProject(string locationOfCode)
        {
            var command = "dotnet";
            var args = "new webapi";
            var process = new ProcessExecutor();

            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, locationOfCode, message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> RestoreDotnetPackages(string locationOfCode)
        {
            var command = "dotnet";
            var args = "restore";
            var process = new ProcessExecutor();

            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, locationOfCode, message => t.TrySetResult(message));
                return t.Task;
            });
        }

        public static Task<List<ConsoleMessage>> BuildDotnetBinary(string locationOfCode)
        {
            var command = "dotnet";
            var args = "publish -c Release -o out";
            var process = new ProcessExecutor();

            return Task<List<ConsoleMessage>>.Run(() =>
            {
                var t = new TaskCompletionSource<List<ConsoleMessage>>();
                process.ExecuteCLIWithResult(command, args, locationOfCode, message => t.TrySetResult(message));
                return t.Task;
            });
        }
    }
}