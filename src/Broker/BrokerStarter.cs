using System.Threading.Tasks;
using Broker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Broker
{
    public static class BrokerStarter
    {
        [FunctionName("RequestQuotations")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function,  "post")]
            LoanRequest loanRequest,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"About to start loan request for SSN: {loanRequest.SSN}");
            var orchestrationId = await starter.StartNewAsync(nameof(BrokerOrchestration.RunOrchestrator), loanRequest);

            return new OkObjectResult(starter.CreateHttpManagementPayload(orchestrationId));
        }
    }
}