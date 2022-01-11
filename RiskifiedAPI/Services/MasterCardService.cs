using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RiskifiedAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiskifiedAPI
{
    public class MasterCardService : Service
    {


        public class responseData
        {
            public string decline_reason { get; set; }

        }

        public MasterCardService(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {
        }


        //get value from config file (appsettings.json)
        public override string url => _configuration["Companies:MasterCard:Url"];

        public override string headerName => _configuration["Companies:MasterCard:HeaderName"];

        public override string headerValue => _configuration["Companies:MasterCard:HeaderValue"];




        //create json body for this spesific request
        public override string buildPaymentBody(Payment payment)
        {

            var paymentJsonData = new
            {
                first_name = payment.fullName.Split(' ')[0],
                last_name = payment.fullName.Split(' ')[1],
                card_number = payment.creditCardNumber,
                expiration = payment.expirationDate,
                cvv = payment.cvv,
                charge_amount = payment.amount
            };

            string jsonString = JsonSerializer.Serialize(paymentJsonData);
            return jsonString;
        }



        protected override bool handleResponse(string responseContent)
        {
            if (responseContent == "OK") return true;
            responseData res =
              JsonSerializer.Deserialize<responseData>(responseContent);

        //    addToCache(res.decline_reason, currentMerchantName);
            return false;
        }



        protected override bool handleError(string s)
        {
            responseData res =
           JsonSerializer.Deserialize<responseData>(s);
            return true;
        }
    }
}
