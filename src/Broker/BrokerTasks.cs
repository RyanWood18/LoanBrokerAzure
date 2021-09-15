using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Broker.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Broker
{
    public static class BrokerTasks
    {
        private static readonly HttpClient httpClient = new HttpClient();
        [FunctionName(nameof(GetCreditScore))]
        public static async Task<CreditScore> GetCreditScore([ActivityTrigger] string ssn, ILogger log)
        {
            var creditBureau = Environment.GetEnvironmentVariable("CreditBureauUrl");
            var requestUri = string.Format(creditBureau, ssn);
            var request = await httpClient.GetAsync(requestUri);

            var score = JsonSerializer.Deserialize<CreditScore>(await request.Content.ReadAsStringAsync(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            log.LogInformation($"Received credit score of {score.Score}");
            return score;
        }

        [FunctionName(nameof(GetQuotations))]
        public static async Task<LoanRequestEntity> GetQuotations([ActivityTrigger] LoanRequestContext request,
            [ServiceBus("LoanQuoteRequest", Connection = "ServiceBusConnection")]
            IAsyncCollector<LoanRequestWithCreditScore> loanMessageCollector, [Table("LoanRequests")] IAsyncCollector<LoanRequestEntity> loanRequestCollector)
        {
            var requestId = Guid.NewGuid().ToString();
            var loanRequest = new LoanRequestEntity
            {
                PartitionKey = "Requests",
                RowKey = requestId,
                OrchestrationId = request.OrchestrationId,
                NotificationEmail = request.LoanRequest.EmailAddress
            };
            
            await loanRequestCollector.AddAsync(loanRequest);
            
            await loanMessageCollector.AddAsync(new LoanRequestWithCreditScore{Amount = request.LoanRequest.Amount, CreditScore = request.CreditScore, RequestId = requestId});

            return loanRequest;
        }

        [FunctionName(nameof(NotifyAllQuotesReceived))]
        public static async Task NotifyAllQuotesReceived([ActivityTrigger] CompletedRequest request,
            [SendGrid] IAsyncCollector<SendGridMessage> collector)
        {
            var message = CreateMessage(request.EmailAddress, request.Quotes);
            await collector.AddAsync(message);
        }

        [FunctionName(nameof(NotifyQuotesPartiallyReceived))]
        public static async Task NotifyQuotesPartiallyReceived([ActivityTrigger] LoanRequestEntity request,
            [Table("Quotations")] CloudTable quotesTable, [SendGrid] IAsyncCollector<SendGridMessage> collector)
        {
            var quotesQuery = new TableQuery<QuotationAggregator.QuotationEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, request.RowKey));
            var quoteResults = await quotesTable.ExecuteQuerySegmentedAsync(quotesQuery, null);
            var quotes = quoteResults.Results;

            var message = CreateMessage(request.NotificationEmail,
                quotes.Select(x => new QuotationAggregator.Quotation {BankId = x.RowKey, Rate = x.Rate}).ToList());
            await collector.AddAsync(message);
        }

        private static SendGridMessage CreateMessage(string toAddress, List<QuotationAggregator.Quotation> quotes)
        {
            var message = new SendGridMessage();
            message.AddTo(toAddress);
            message.SetSubject(quotes.Count > 0 ? "Quotes Received" : "An update on your loan quote request");
            message.SetFrom(Environment.GetEnvironmentVariable("FromEmailAddress"));
            message.AddContent("text/html", EmailBuilder.BuildEmail(quotes));
            return message;
        }
    }

    public class LoanRequestEntity : TableEntity
    {
        public string OrchestrationId { get; set; }
        public string NotificationEmail { get; set; }
    }
}