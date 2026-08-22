using data.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User")]
    [EndpointName("create payment sessionId")]
    [EndpointDescription(
        "This function is user when start submit order to create session id when using strip as payment getway")]
    public async Task<IActionResult> CreateSession([FromBody] PaymentRequirementData paymentRequirementData)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = paymentRequirementData.Amount * 100,
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return Ok(new { client_secret = paymentIntent.ClientSecret });
    }
}