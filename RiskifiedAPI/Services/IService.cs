using RiskifiedAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedAPI
{
    public interface IService
    {
        
        Task<bool> sendPayment(Payment payment, string merchantName);
        
        //implemented by each service
      //  string buildPaymentBody(Payment payment);

       // bool handleResponse(string response);


    }
}
