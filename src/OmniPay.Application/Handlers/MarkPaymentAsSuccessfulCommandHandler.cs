using Microsoft.Extensions.Logging;
using OmniPay.Application.Commands;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class MarkPaymentAsSuccessfulCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<MarkPaymentAsSuccessfulCommandHandler> _logger;

    public MarkPaymentAsSuccessfulCommandHandler(
        IPaymentRepository paymentRepository,
        ILogger<MarkPaymentAsSuccessfulCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Payment> HandleAsync(
        MarkPaymentAsSuccessfulCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking payment {PaymentId} as successful", command.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(
            command.PaymentId,
            cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Mark successful failed. Payment {PaymentId} not found", command.PaymentId);
            throw new KeyNotFoundException(
                $"Payment with ID '{command.PaymentId}' was not found.");
        }

        payment.MarkAsSuccessful();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment {PaymentId} marked as successful", command.PaymentId);

        return payment;
    }
}
