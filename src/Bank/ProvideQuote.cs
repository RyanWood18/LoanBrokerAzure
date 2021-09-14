using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Bank
{
    public static class ProvideQuote
    {
        private static readonly string BankId = Environment.GetEnvironmentVariable("BankId");
        private static readonly decimal MaxLoanAmount = decimal.Parse(Environment.GetEnvironmentVariable("MaxLoanAmount")??"0");
        private static readonly int MinimumCreditScore =
            int.Parse(Environment.GetEnvironmentVariable("MinimumCreditScore") ?? "1000");
        private static readonly double BaseRate = double.Parse(Environment.GetEnvironmentVariable("BaseRate") ?? "0");
        
        [FunctionName("ProvideQuote")]
        public static async Task Run([ServiceBusTrigger("LoanQuoteRequest", subscriptionName: "%SubscriptionName%", Connection = "ServiceBusConnection")]QuotationRequest request, 
            ILogger log, 
            [ServiceBus("LoanQuotation", Connection="ServiceBusConnection")] IAsyncCollector<Quotation> collector)
        {
            if (request.Amount > MaxLoanAmount || request.CreditScore.Score<MinimumCreditScore)
            {
                log.LogInformation($"Loan criteria for request {request.RequestId} rejected by bank {BankId} ");
                return;
            }

            var rate = CalculateRate(request.CreditScore.Score);
            log.LogInformation($"Loan criteria for request {request.RequestId} accepted at {rate} by bank {BankId} ");
            await collector.AddAsync(new Quotation {Status = "Accepted", Rate = rate, BankId = BankId, RequestId = request.RequestId});
        }

        private static double CalculateRate(int creditScore)
        {
            var random = new Random();
            return BaseRate * random.Next() * ((1000 - creditScore) / 100f);
        }
    }

    public class QuotationRequest
    {
        public decimal Amount { get; set; }
        public string RequestId { get; set; }
        public CreditScore CreditScore { get; set; }
    }

    public class CreditScore
    {
        public int Score { get; set; }
        public int History { get; set; }
    }

    public class Quotation
    {
        public string Status { get; set; }
        public double? Rate { get; set; }
        public string BankId { get; set; }
        public string RequestId { get; set; }
    }
}
