using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IPaymentService
{
    public interface IPayment
    {
        // Application/IPaymentService/IPaymentService.cs
       
            Task<PaymentResultDto> ProcessPaymentAsync(PaymentRequestDto request, string userid);
    }
}
