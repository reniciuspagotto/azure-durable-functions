using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpFunction2
{
    public static class RandomIntFunction
    {
        [FunctionName("GenerateRandomNumber")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Random rnd = new Random();
            var response = "";

            for (int j = 0; j < 2; j++)
            {
                response += $"{rnd.Next()}";
            }

            return new OkObjectResult($"Randon number: {response}");
        }
    }
}
