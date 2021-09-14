using System.Collections.Generic;

namespace Broker.Entities
{
    public class CompletedRequest
    {
        public string EmailAddress { get; set; }
        public List<QuotationAggregator.Quotation> Quotes { get; set; }
    }
}