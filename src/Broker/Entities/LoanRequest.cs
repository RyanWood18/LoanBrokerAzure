namespace Broker.Entities
{
    public class LoanRequest
    {
        public  decimal Amount { get; set; }
        public string SSN { get; set; }
        public string EmailAddress { get; set; }
    }
}