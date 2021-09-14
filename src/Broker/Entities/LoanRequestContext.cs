namespace Broker.Entities
{
    public class LoanRequestContext
    {
        public string OrchestrationId { get; set; }
        public LoanRequest LoanRequest { get; set; }
        public CreditScore CreditScore { get; set; }
    }
}