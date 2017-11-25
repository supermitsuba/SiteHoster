namespace SiteHoster.Common.Services
{
    using System.Text;
    using RabbitMQ.Client;
    using System;
    using System.IO;
    using RabbitMQ.Client.Events;
    using System.Collections.Generic;
    using SiteHoster.Common.Models;

    public class RabbitMQReceiver : IDisposable
    {
        private static object myLock = new object();
        public bool IsStarted { get; private set; } 
        private readonly IConnection conn;
        private readonly string queueName;
        private IModel model; 
        private EventingBasicConsumer consumer; 
        private Action<RabbitMessage> messageReceivedAction;

        public RabbitMQReceiver(string url, string queueName)
        {
            var factory = new ConnectionFactory();
            factory.AutomaticRecoveryEnabled = true;
            factory.Uri = new Uri(url);
            this.conn = factory.CreateConnection();
            this.queueName = queueName;
        }
    
        public void Start(Action<RabbitMessage> messageReceivedAction) { 
            if (!IsStarted) { 
                IsStarted = true; 
                model = conn.CreateModel(); 
                this.consumer = new EventingBasicConsumer(model); 
                consumer.Received += consumer_Received;
                
                IDictionary<string, object> dict = null; 
                
                this.messageReceivedAction = messageReceivedAction;
                model.QueueDeclare(this.queueName, true, false, false, dict); 
                model.BasicQos(0, 1, false); 
                model.BasicConsume(this.queueName, false, consumer); 
            } 
        } 

        void consumer_Received(object sender, BasicDeliverEventArgs e) { 
            try
            {
                lock(myLock)
                {
                    var message = Encoding.UTF8.GetString(e.Body); 
                    var myMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitMessage>(message);
                    messageReceivedAction.Invoke(myMessage);
                    model.BasicAck(e.DeliveryTag, false);
                }
            }
            finally
            {
               
            }
        }

        public void Stop()
        {
            if(IsStarted) {
                IsStarted = false;
                model.Close();
                model.Dispose();
                consumer.Received -= consumer_Received;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Stop();
                    conn.Close();
                    conn.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RabbitMQClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}