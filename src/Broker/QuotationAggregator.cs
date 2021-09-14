using System.Linq;
using System.Threading.Tasks;
using Broker.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Broker
{
    public static class QuotationAggregator
    {
        [FunctionName("CollectQuotations")]
        public static async Task Run([ServiceBusTrigger("LoanQuotation", Connection = "ServiceBusConnection")]Quotation quote, 
            [Table("Quotations")]CloudTable cloudTable, 
            [Table("LoanRequests", "Requests", "{RequestId}")] LoanRequestEntity loanRequest, 
            ILogger log, [DurableClient] IDurableClient durableClient)
        {
            await cloudTable.ExecuteAsync(TableOperation.Insert(new QuotationEntity
                {Rate = quote.Rate, PartitionKey = quote.RequestId, RowKey = quote.BankId}));
            
            TableQuery<QuotationEntity> query = new TableQuery<QuotationEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, quote.RequestId));

            var queryResults = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var quotations = queryResults.Results;

            if (quotations.Count()>=2)
            {
                var completedQuotation = new CompletedRequest
                {
                    EmailAddress = loanRequest.NotificationEmail,
                    Quotes = quotations.Select(x => new Quotation {BankId = x.RowKey, Rate = x.Rate}).ToList()
                };
                await durableClient.RaiseEventAsync(loanRequest.OrchestrationId, "QuotationsReceived", completedQuotation);
            }
        }
        
        public class Quotation
        {
            public double Rate { get; set; }
            public string BankId { get; set; }
            public string RequestId { get; set; }
        }

        public class QuotationEntity : TableEntity
        {
            public double Rate { get; set; }
        }
    }
}
