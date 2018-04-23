using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecondWebApplication.Controllers;

namespace SecondWebApplication
{
    public class SendToRabbitmq
    {
        IConnection conn;
        IModel model;
        EventingBasicConsumer consumer;
        IBasicProperties properties;
        string queuename;
        
        string returningdata;
        public SendToRabbitmq(ILogger<ValuesController> log)
        {
            

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var conn = factory.CreateConnection();

            model = conn.CreateModel();

            properties = model.CreateBasicProperties();

            properties.Persistent = true;

            var correlationid = Guid.NewGuid().ToString();

            queuename = model.QueueDeclare().QueueName;

            properties.CorrelationId = correlationid;

            properties.ReplyTo = queuename;

            consumer =new EventingBasicConsumer(model);

            consumer.Received += (channel, ea) =>
            {
                var body = Encoding.ASCII.GetString(ea.Body);
                log.LogError($"Body Recived responce - {body}");
                if (ea.BasicProperties.CorrelationId == properties.CorrelationId)
                {
                    returningdata = body;
                    log.LogError($"Sucsses Recive - {body}");
                }
            };
            model.BasicConsume(queuename, true, consumer);

        }

        public void Publisher(string body, ILogger logger)
        {                                                                                                   
                model.BasicPublish("MyExchange", "", properties, (Encoding.ASCII.GetBytes(body)));
                
                logger.LogError($"Message is Send - {body}");

        }

        public string Getmassege()
        {                                           
            return returningdata;
        }
    }
}
