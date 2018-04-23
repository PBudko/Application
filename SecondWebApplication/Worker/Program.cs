using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;
using System;
using System.Text;

namespace Worker
{
    class Program
    {
        static Logger logger;
        static void Main(string[] args)
        {
            Create_LoggerConfig(ref logger);
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                channel.QueueBind("firstqueue", "MyExchange", "");
                var consumar = new EventingBasicConsumer(channel);

                try
                {
                    consumar.Received += (model, ea) =>
                    {
                        var responsemessage = Encoding.ASCII.GetString(ea.Body);

                        Console.WriteLine($"Recived massege - {responsemessage}");

                        channel.BasicAck(ea.DeliveryTag, false);

                        responsemessage = $"Name: Valera, Age: 31";

                        var replyproperties = channel.CreateBasicProperties();

                        replyproperties.CorrelationId = ea.BasicProperties.CorrelationId;

                        channel.BasicPublish("", ea.BasicProperties.ReplyTo, replyproperties, (Encoding.ASCII.GetBytes(responsemessage)));

                        Console.WriteLine($"Send Message - {responsemessage}");
                    };
                    channel.BasicConsume("firstqueue", false, consumar);
                    logger.Information($"Information Recived succes {DateTime.Now}");
                }
                catch (Exception ex)
                {

                    logger.Error($"Error in Recive {ex.Message} {DateTime.Now}");
                }
                

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public static void Create_LoggerConfig(ref Logger logger)
        {
            var config = new LoggerConfiguration();
            config.MinimumLevel.Information();
            config.WriteTo.File(new JsonFormatter(), "ClientLogs.txt");

            logger = config.CreateLogger();
        }
    }
}
