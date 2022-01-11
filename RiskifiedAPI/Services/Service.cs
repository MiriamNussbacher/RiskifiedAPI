using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RiskifiedAPI.Entities;


namespace RiskifiedAPI
{
    public abstract class Service : IService
    {
        public abstract string url { get; }
        public abstract string headerName { get; }
        public abstract string headerValue { get; }//implemented in every service, open to different headers in the future

        public  string currentMerchantName { get; set; }

        protected readonly IMemoryCache _cache;
        protected readonly IConfiguration _configuration;


        protected Service(IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _configuration = configuration;
        }

      

        public abstract string buildPaymentBody(Payment payment);
        protected abstract bool handleResponse(string response);
        protected virtual bool handleError(string s) => false;

        public async Task<bool> sendPayment(Payment payment, string merchantName)
        {
            bool res = true;
            currentMerchantName = merchantName;
            //get data from the client and prepare to send to specific company.
            //implementation different for each.
            string paymentData = buildPaymentBody(payment);

            //define retry policy. take num of retries from config. if missing, default is 3
            var maxRetryAttempts = _configuration.GetValue<int>("NumOfRetries",3);

            
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(maxRetryAttempts, i => TimeSpan.FromSeconds(i * i));
           
            var client = new HttpClient();
            HttpResponseMessage responseMessage;
            
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    //send to company
                    HttpContent content = new StringContent(paymentData, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add(headerName, headerValue);
                    responseMessage = await client.PostAsync(url, content);
                    responseMessage.EnsureSuccessStatusCode();
                    string contentString= await responseMessage.Content.ReadAsStringAsync();
                    
                    res = handleResponse(contentString);//return true or false based on declined or success
                    //implemented by eash service based on company response
                   

                  });
                return res;

            }
            catch(HttpRequestException ex)//if all failed, catch only http exceptions and return false
            {

              //  handleError(ex.Message); maybe save for duture use
                return false;
            }           
        }

        //NOT IN USE MEANWHILE
        public  void addToCache(string declineMessage, string merchatIdnt)
        {
            
            CacheItem records =  _cache.Get<CacheItem>("declines");//has to be created on startup
            DeclineRecords record = records.items.Where(merchant => merchant.merchatIdentifier == merchatIdnt).FirstOrDefault();
            if (record == null)
            {//new merchat
                record = new DeclineRecords();
                record.merchatIdentifier = currentMerchantName;//merchatIdnt;
                record.declinesPerUser.Add(new DeclineSum() { count = 1, reason = declineMessage });
            }
            else
            {//existing merchant, new decline reason
                DeclineSum declineSum= record.declinesPerUser.Where(decline => decline.reason == declineMessage).FirstOrDefault();
                if (declineSum == null)
                {
                    declineSum = new DeclineSum(){ count = 1, reason = declineMessage };
                    record.declinesPerUser.Add(declineSum);
                }
                else//existing merchant, and existing reason, just increase count.
                {
                    declineSum.count++;
                }
            }
            _cache.Set<CacheItem>("declines", records);//return to cache

        }

        
    }
}




