using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableFunctionFanInFanOutOrchestrator
{
    public static class FunctionFanInFanOutOrchestrator
    {
        public static string urlRandomString = "http://localhost:7017/api/GenerateRandomString";
        public static string urlRandomNumber = "http://localhost:7023/api/GenerateRandomNumber";

        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("FunctionFanInFanOutOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("FunctionFanInFanOutOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var tasks = new List<Task<DurableHttpResponse>>();
            tasks.Add(context.CallHttpAsync(HttpMethod.Get, new Uri(urlRandomString)));
            tasks.Add(context.CallHttpAsync(HttpMethod.Get, new Uri(urlRandomNumber)));

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result.Content).ToList();
        }
    }
}