namespace SiteHosterSite.Services.Commands
{
    using System.Threading.Tasks;
    using SiteHoster.Common.Models;
    
    public abstract class Command
    {
        protected RabbitMessage receiver;

        public Command(RabbitMessage receiver){
            this.receiver = receiver;
        }

        public abstract Task Execute();
    }
}