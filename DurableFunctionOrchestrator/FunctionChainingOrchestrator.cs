using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableFunctionChainingOrchestrator
{
    public static class FunctionChainingOrchestrator
    {
        public static string urlRandomString = "http://localhost:7017/api/GenerateRandomString";
        public static string urlRandomNumber = "http://localhost:7023/api/GenerateRandomNumber";

        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("FunctionChainingOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("FunctionChainingOrchestrator")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Implementation without activity function
            DurableHttpResponse response1 = await context.CallHttpAsync(HttpMethod.Get, new Uri(urlRandomString));
            DurableHttpResponse response2 = await context.CallHttpAsync(HttpMethod.Get, new Uri(urlRandomNumber));
            outputs.Add(response1.Content);
            outputs.Add(response2.Content);

            // Implementation using activity function
            outputs.Add(await context.CallActivityAsync<string>("Function_GetRandomString", null));
            outputs.Add(await context.CallActivityAsync<string>("Function_GetRandomNumber", null));

            return outputs;
        }

        [FunctionName("Function_GetRandomString")]
        public static async Task<string> GetRandomString([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello");

            using var client = new HttpClient();
            var content = await client.GetStringAsync("http://localhost:7017/api/GenerateRandomString");

            return content;
        }

        [FunctionName("Function_GetRandomNumber")]
        public static async Task<string> GetRandomNumber([ActivityTrigger] string name, ILogger log)
        {
            using var client = new HttpClient();
            var content = await client.GetStringAsync("http://localhost:7023/api/GenerateRandomNumber");

            return content;
        }
    }
}