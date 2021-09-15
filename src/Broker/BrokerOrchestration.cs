using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broker.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Broker
{
    public static class BrokerOrchestration
    {
        [FunctionName("RunOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log);
            var loanRequest = context.GetInput<LoanRequest>();
            var outputs = new List<string>();
            log.LogInformation("Fetching Credit Score");
            var creditScore =
                await context.CallActivityAsync<CreditScore>(nameof(BrokerTasks.GetCreditScore), loanRequest.SSN);
            var requestContext = new LoanRequestContext
                {CreditScore = creditScore, LoanRequest = loanRequest, OrchestrationId = context.InstanceId};
            log.LogInformation("Requesting Quotations");
            var request = await context.CallActivityAsync<LoanRequestEntity>(nameof(BrokerTasks.GetQuotations), requestContext);
            
            try
            {
                var completion = await context.WaitForExternalEvent<CompletedRequest>("QuotationsReceived", TimeSpan.FromSeconds(60));
                log.LogInformation("Two or more quotations received");
                await context.CallActivityAsync(nameof(BrokerTasks.NotifyAllQuotesReceived), completion);
            }
            catch (TimeoutException)
            {
                log.LogWarning("Failed to receive desired number of quotes");
                await context.CallActivityAsync(nameof(BrokerTasks.NotifyQuotesPartiallyReceived), request);
            }
            return outputs;
        }
    }
}