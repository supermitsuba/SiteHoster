namespace SiteHosterSite.Services
{
    using System.Diagnostics;

    public class DotNetService
    {
        public void RestoreDotnetPackages(string locationOfCode)
        {
            var command1 = "dotnet";
            var args1 = "restore";
            var process = new Process(); 
            process.StartInfo.FileName = command1; 
            process.StartInfo.Arguments = args1;
            process.StartInfo.WorkingDirectory = locationOfCode;
            process.Start(); 
            process.WaitForExit(); //this limit might need to be changed
        }

        public void BuildDotnetBinary(string locationOfCode)
        {
            var command2 = "dotnet";
            var args2 = "publish -c Release -o releasebin";
            var process2 = new Process(); 
            process2.StartInfo.FileName = command2; 
            process2.StartInfo.Arguments = args2;
            process2.StartInfo.WorkingDirectory = locationOfCode;
            process2.Start();
            process2.WaitForExit();
        }
    }
}