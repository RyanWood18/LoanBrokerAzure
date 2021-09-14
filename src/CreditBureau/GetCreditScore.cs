using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreditBureau
{
    public static class GetCreditScore
    {
        private static readonly Regex SsnRegex = new Regex("^\\d{3}-\\d{2}-\\d{4}$");
        
        [FunctionName("GetCreditScore")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCreditScore/{ssn}")] string ssn,
            ILogger log)
        {
            log.LogInformation("Requesting Credit Score.");

            if (!SsnRegex.IsMatch(ssn))
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(new {SSN = ssn, Score = GetRandom(300,900), History = GetRandom(1,30)});
        }

        private static int GetRandom(int min, int max)
        {
            var random = new Random();
            return min + (int)Math.Floor((double) random.Next() * (max - min));
        }
    }
}
