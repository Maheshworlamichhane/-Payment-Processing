using Application.IPaymentService;
using Domain.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;  
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPayment _paymentService;

        public PaymentsController(IPayment paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _paymentService.ProcessPaymentAsync(request, userId);
                return Accepted(result); 
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }
    }
}

