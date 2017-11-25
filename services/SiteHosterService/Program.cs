using System;
using System.Threading;
using Newtonsoft.Json;
using SiteHoster.Common.Models;
using SiteHoster.Common.Services;
using SiteHosterSite.Services.Commands;

namespace SiteHosterSite
{
    class Program
    {
        static string httpUrl = "http://localhost:5000";
        static string url = "amqp://guest:guest@192.168.10.115";
        static string queueName = "sitehoster";

        static void Main(string[] args)
        {
            var messageReceiver = new RabbitMQReceiver(url, queueName);
            messageReceiver.Start( ProcessMessage );

            while(true) { Thread.Sleep(5000); }

            messageReceiver.Dispose();
        }

        static void ProcessMessage(RabbitMessage message)
        {
            Command command = null;

            switch(message.RabbitMessageType)
            {
                case MessageType.BuildDocker:
                    command = new BuildDockerCommand(message, 
                        new Services.DockerService(), 
                        new DiscoveryServiceClient(httpUrl));
                    break;
                case MessageType.BuildDotNet:
                    command = new BuildDotNetCommand(message, 
                        new Services.DotNetService(), 
                        new DiscoveryServiceClient(httpUrl));
                    break;
                default:
                    // unable to process message
                    Console.WriteLine($"[Warn] - Could not process message: {JsonConvert.SerializeObject(message)}");
                    break;
            }

            if(command != null)
            {
                var task = command.Execute();
                task.Wait();
            }
        }
    }
}
