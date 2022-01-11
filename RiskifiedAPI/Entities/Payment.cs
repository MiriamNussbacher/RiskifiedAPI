using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedAPI.Entities
{
    public class Payment
    {
        [Required]
        public string fullName { get; set; }
        [Required]
        public string creditCardNumber { get; set; }
        [Required]
        public string creditCardCompany { get; set; }
        [Required]
        public string expirationDate { get; set; }
        [Required]
        public string cvv { get; set; }
        [Required]
        public Decimal amount { get; set; }


    }
}
