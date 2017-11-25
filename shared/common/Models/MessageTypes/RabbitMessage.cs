namespace SiteHoster.Common.Models
{    
    public class RabbitMessage
    {
        public MessageType RabbitMessageType {get;set;}

        public string Message {get;set;}
    }
}