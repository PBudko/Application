using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecondWebApplication.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ILogger<ValuesController> log { get; set; }
        
        public ValuesController(ILogger<ValuesController> logger)
        {
            this.log = logger;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Person value)
        {
            var somemessage = "";

            try
            {
                var sender = new SendToRabbitmq(log);
           
                sender.Publisher($"Name: {value.Name}, Age: {value.Age.ToString()}",log);
                

                var starting = DateTime.Now;

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                while (string.IsNullOrEmpty(somemessage) || !(stopwatch.ElapsedMilliseconds>2000))
                {
                     somemessage = sender.Getmassege();    Thread.Sleep(TimeSpan.FromMilliseconds(500));           
                }

                if (stopwatch.ElapsedMilliseconds > 3000)
                {
                    log.LogError("Error To Match Waiting");
                    return new ObjectResult("Error ToMath Time waiting");
                }
                stopwatch.Reset();

                log.LogInformation($"Succes build in Post Request {DateTime.Now}");
                
            }
            catch (Exception ex)
            {
                log.LogError($"Error {ex.Message}");
            }
            return new ObjectResult(somemessage);
            
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
