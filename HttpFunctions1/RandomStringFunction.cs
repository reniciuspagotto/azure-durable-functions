using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpFunction1
{
    public static class RandomStringFunction
    {
        [FunctionName("GenerateRandomString")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Random rand = new Random();
            int stringlen = rand.Next(10, 15);
            int randValue;
            string str = "";
            char letter;

            for (int i = 0; i < stringlen; i++)
            {
                randValue = rand.Next(0, 26);
                letter = Convert.ToChar(randValue + 65);
                str = str + letter;
            }

            return new OkObjectResult($"Randon String: {str}");
        }
    }
}
