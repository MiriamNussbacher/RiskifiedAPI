using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RiskifiedAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiskifiedAPI
{
    public class VisaService : Service
    {

        public VisaService(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {

        }

        public class responseData
        {
            public string chargeResult { get; set; }
            public string resultReason { get; set; }

        }
     

        //get value from config file (appsettings.json)
        public override string url => _configuration["Companies:Visa:Url"];

        public override string headerName => _configuration["Companies:Visa:HeaderName"];

        public override string headerValue => _configuration["Companies:Visa:HeaderValue"];

      //  public override string currentMerchantName => "";


        //create json body for this spesific request
        public override string buildPaymentBody(Payment payment)
        {

            var paymentJsonData = new
            {
                fullName = payment.fullName,
                number = payment.creditCardNumber,
                expiration = payment.expirationDate,
                cvv = payment.cvv,
                totalAmount = payment.amount
            };

            string jsonString = JsonSerializer.Serialize(paymentJsonData);
            return jsonString;
        }

        protected override bool handleResponse(string s)
        {
            responseData res =
          JsonSerializer.Deserialize<responseData>(s);
            if (res.chargeResult == "Success")
                return true;
            return false;
        }


     
    }
}
