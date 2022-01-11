using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RiskifiedAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static RiskifiedAPI.Startup;

namespace RiskifiedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeController : ControllerBase
    {

        ServiceResolver serviceResolver;
        IService service;

        public ChargeController(ServiceResolver serviceResolver)
        {
            this.serviceResolver = serviceResolver;
        }

        [HttpPost]
        public async Task<IActionResult> addPayment([FromBody] Payment payment)
        {


            StringValues merchantIdentifier;
                if (!Request.Headers.TryGetValue("merchant-identifier", out merchantIdentifier))
                    return BadRequest();

                service = serviceResolver(payment.creditCardCompany);
                bool result = await service.sendPayment(payment, merchantIdentifier.ToString());
           
            if (result)
                return Ok();
            else
            {
                return Ok(new
                {
                    error = "Card Declined"

                });
            }
  
        }
    }
}
