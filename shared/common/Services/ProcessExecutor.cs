namespace SiteHoster.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using SiteHoster.Common.Models;

    public class ProcessExecutor
    {
        public ProcessExecutor()
        {
            this.messagesForCallback = new List<ConsoleMessage>();
        }

        public static void ExecuteCLI(string command, string args, string workingDirectory)
        {
            using(var process = new Process())
            {
                process.StartInfo.FileName = command; 
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.Start(); 
                process.WaitForExit(15000); //this limit might need to be changed
            }
        }

        List<ConsoleMessage> messagesForCallback;
        public void ExecuteCLIWithResult(string command, string args, string workingDirectory, Action<List<ConsoleMessage>> callback)
        {
            using(var process = new Process())
            {
                process.StartInfo.FileName = command; 
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = workingDirectory;

                process.Start();
                process.ErrorDataReceived += new DataReceivedEventHandler(ErrorOutputReceived);
                process.OutputDataReceived += new DataReceivedEventHandler(OutputReceived);
                process.EnableRaisingEvents = true;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(); //this limit might need to be changed
                process.ErrorDataReceived -= new DataReceivedEventHandler(ErrorOutputReceived);   
                process.OutputDataReceived -= new DataReceivedEventHandler(OutputReceived);
            }

            callback(this.messagesForCallback);
        }

        private void ErrorOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.Data)) return;

            this.messagesForCallback.Add(new ConsoleMessage(){
                Message = e.Data,
                IsError = true
            });
        }

        private void OutputReceived(object sender, DataReceivedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.Data)) return;
            
            this.messagesForCallback.Add(new ConsoleMessage(){
                Message = e.Data,
                IsError = false
            });
        }
    }
}   