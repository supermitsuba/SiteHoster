namespace SiteHosterSite.Services.Commands
{
    using SiteHoster.Common.Models;
    
    public abstract class Command
    {
        protected RabbitMessage receiver;

        public Command(RabbitMessage receiver){
            this.receiver = receiver;
        }

        public abstract void Execute();
    }
}