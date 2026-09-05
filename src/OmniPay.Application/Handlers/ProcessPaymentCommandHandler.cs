using Microsoft.Extensions.Logging;
using OmniPay.Application.Commands;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class ProcessPaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _logger = logger;
    }

    public async Task<Payment> HandleAsync(
        ProcessPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment {PaymentId}", command.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Process failed. Payment {PaymentId} was not found", command.PaymentId);
            throw new KeyNotFoundException($"Payment with ID '{command.PaymentId}' was not found.");
        }

        var gatewayResult = await _paymentGateway.ProcessPaymentAsync(payment, cancellationToken);

        if (gatewayResult)
        {
            payment.MarkAsSuccessful();
            _logger.LogInformation("Payment {PaymentId} marked as successful after processing", command.PaymentId);
        }
        else
        {
            payment.MarkAsFailed();
            _logger.LogInformation("Payment {PaymentId} marked as failed after processing", command.PaymentId);
        }

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }
}
