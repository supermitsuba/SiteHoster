namespace SiteHosterSite.Services
{
    using System.Diagnostics;

    public class DockerService 
    {
        public void BuildDockerImage(string nameOfApplication, string directoryOfDockerfile)
        {
            var command1 = "docker";
            var args1 = $"build -t supermitsuba/{nameOfApplication}:1 . --no-cache";
            var process = new Process(); 
            process.StartInfo.FileName = command1; 
            process.StartInfo.Arguments = args1;
            process.StartInfo.WorkingDirectory = directoryOfDockerfile;
            process.Start(); 
            process.WaitForExit(); //this limit might need to be changed
        }

        public void RunDockerImage(string nameOfApplication, string directoryOfDockerfile)
        {
                        // set port number
            var command2 = "docker";
            var args2 = $"run -d -p 7000:5000 supermitsuba/{nameOfApplication}:1";
            var process2 = new Process(); 
            process2.StartInfo.FileName = command2; 
            process2.StartInfo.Arguments = args2;
            process2.StartInfo.WorkingDirectory = directoryOfDockerfile;
            process2.Start();
            process2.WaitForExit();
        }
    }
}