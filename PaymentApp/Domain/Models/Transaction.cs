using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace Domain.Models
{
    public class Transaction
    {
        public Guid TransactionID { get; set; }

        //[ForeignKey("User")]
        public string UserId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Timestamp { get; set; }

       // public IdentityUser User { get; set; }  // Navigation property
    }
}

