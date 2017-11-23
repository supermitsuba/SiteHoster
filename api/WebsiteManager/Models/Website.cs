namespace WebsiteManager.Models
{
    using System;

    public class Website
    {
        public string Name { get;set;}
        public string Description {get;set;}
        public WebsiteStatus Status {get;set;}
        public string Path {get;set;}
        public Uri DockerUrl {get;set;}
    }
}