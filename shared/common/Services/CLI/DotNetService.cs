namespace SiteHosterSite.Services
{
    using System.Diagnostics;
    using SiteHoster.Common.Services;

    public class DotNetService
    {
        public void RestoreDotnetPackages(string locationOfCode)
        {
            var command1 = "dotnet";
            var args1 = "restore";
            ProcessExecutor.ExecuteCLI(command1, args1, locationOfCode);
        }

        public void BuildDotnetBinary(string locationOfCode)
        {
            var command2 = "dotnet";
            var args2 = "publish -c Release -o releasebin";
            ProcessExecutor.ExecuteCLI(command2, args2, locationOfCode);
        }
    }
}