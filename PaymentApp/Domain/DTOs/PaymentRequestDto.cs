using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    // Application/DTOs/PaymentRequestDto.cs
    public class PaymentRequestDto
    {
        public string CustomerName { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
    }

    // Application/DTOs/PaymentResultDto.cs
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }

}
