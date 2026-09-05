using Microsoft.Extensions.Logging;
using OmniPay.Application.Commands;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class MarkPaymentAsFailedCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<MarkPaymentAsFailedCommandHandler> _logger;

    public MarkPaymentAsFailedCommandHandler(
        IPaymentRepository paymentRepository,
        ILogger<MarkPaymentAsFailedCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Payment> HandleAsync(
        MarkPaymentAsFailedCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking payment {PaymentId} as failed", command.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(
            command.PaymentId,
            cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Mark failed failed. Payment {PaymentId} not found", command.PaymentId);
            throw new KeyNotFoundException(
                $"Payment with ID '{command.PaymentId}' was not found.");
        }

        payment.MarkAsFailed();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment {PaymentId} marked as failed", command.PaymentId);

        return payment;
    }
}
