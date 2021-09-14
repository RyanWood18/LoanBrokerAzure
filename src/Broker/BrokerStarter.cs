using System.Net.Http;
using System.Threading.Tasks;
using Broker.Entities;
using Microsoft.AspNetCore.Http;
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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var ssn = req.Query["ssn"];
            var loanAmount = req.Query["amount"];

            var loanRequest = new LoanRequest {SSN = ssn, Amount = decimal.Parse(loanAmount)};

            var orchestrationId = await starter.StartNewAsync(nameof(BrokerOrchestration.RunOrchestrator), loanRequest);

            return new OkObjectResult(starter.CreateHttpManagementPayload(orchestrationId));
        }
    }
}