using Microsoft.AspNetCore.Mvc;
using OmniPay.Application.Commands;
using OmniPay.Application.DTOs;
using OmniPay.Application.Handlers;
using OmniPay.Application.Queries;

namespace OmniPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly CreatePaymentCommandHandler _createPaymentCommandHandler;
    private readonly GetPaymentByIdQueryHandler _getPaymentByIdQueryHandler;
    private readonly ProcessPaymentCommandHandler _processPaymentCommandHandler;
    private readonly MarkPaymentAsSuccessfulCommandHandler _markPaymentAsSuccessfulCommandHandler;
    private readonly MarkPaymentAsFailedCommandHandler _markPaymentAsFailedCommandHandler;
    private readonly CancelPaymentCommandHandler _cancelPaymentCommandHandler;

    public PaymentsController(
        CreatePaymentCommandHandler createPaymentCommandHandler,
        GetPaymentByIdQueryHandler getPaymentByIdQueryHandler,
        ProcessPaymentCommandHandler processPaymentCommandHandler,
        MarkPaymentAsSuccessfulCommandHandler markPaymentAsSuccessfulCommandHandler,
        MarkPaymentAsFailedCommandHandler markPaymentAsFailedCommandHandler,
        CancelPaymentCommandHandler cancelPaymentCommandHandler)
    {
        _createPaymentCommandHandler = createPaymentCommandHandler;
        _getPaymentByIdQueryHandler = getPaymentByIdQueryHandler;
        _processPaymentCommandHandler = processPaymentCommandHandler;
        _markPaymentAsSuccessfulCommandHandler = markPaymentAsSuccessfulCommandHandler;
        _markPaymentAsFailedCommandHandler = markPaymentAsFailedCommandHandler;
        _cancelPaymentCommandHandler = cancelPaymentCommandHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.MerchantId,
            request.Amount,
            request.Currency,
            request.Description);

        var payment = await _createPaymentCommandHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(payment);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);

        var payment = await _getPaymentByIdQueryHandler.HandleAsync(
            query,
            cancellationToken);

        if (payment is null)
        {
            return NotFound(new { message = "Payment not found." });
        }

        return Ok(payment);
    }

    [HttpPatch("{id:guid}/success")]
    public async Task<IActionResult> MarkAsSuccessful(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkPaymentAsSuccessfulCommand(id);

        var payment = await _markPaymentAsSuccessfulCommandHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(payment);
    }

    [HttpPatch("{id:guid}/fail")]
    public async Task<IActionResult> MarkAsFailed(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkPaymentAsFailedCommand(id);

        var payment = await _markPaymentAsFailedCommandHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(payment);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelPaymentCommand(id);

        var payment = await _cancelPaymentCommandHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(payment);
    }

    [HttpPost("{id:guid}/process")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id);

        var payment = await _processPaymentCommandHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(payment);
    }
}
