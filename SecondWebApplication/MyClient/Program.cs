using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Json;

namespace MyClient
{
    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }


    class Program
    {

       static Logger logger;
        private readonly static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
           
                Create_LoggerConfig(ref logger);
                PostRequest("http://localhost:53132/api/values",logger).Wait();
           
        }
      
      //public static void Create_RabbitMqLogger(ref Logger logger)
      //  {
      //      var config =LoggerConfigurationRabbitMqExtension.RabbitMQ(new Serilog.Configuration.LoggerSinkConfiguration())
      //  }
        public static void Create_LoggerConfig(ref Logger logger)
        {
            var config = new LoggerConfiguration();
            config.MinimumLevel.Information();
            config.WriteTo.File(new JsonFormatter(), "ClientLogs.txt");

            logger = config.CreateLogger();
        }
        static async Task PostRequest(string path,Logger logger)
        {

            var p = new Person() {Age = 30,Name = "Petya" };
            var str = JsonConvert.SerializeObject(p);

            
            try
            {
                using (var client = new HttpClient())
                {
                    using (HttpResponseMessage responce = await client.PostAsync(path, new StringContent(str, Encoding.UTF8, "application/json")))
                    {
                        
                        using (HttpContent q = responce.Content)
                        {
                            string result = await q.ReadAsStringAsync();

                            HttpContentHeaders header = q.Headers;
                            Console.WriteLine($"Request is - {result} Headers = {header.ContentLocation}");
                        }
                    }
                }
                logger.Information($"Message is recived");
            }
            catch (Exception ex)
            {

                logger.Error($"Error Messege request Excepton is {ex.Message} {DateTime.Now}");
            }
            
        }

    }
}
