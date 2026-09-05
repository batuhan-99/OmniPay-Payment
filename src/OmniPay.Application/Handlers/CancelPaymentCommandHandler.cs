using Microsoft.Extensions.Logging;
using OmniPay.Application.Commands;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class CancelPaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<CancelPaymentCommandHandler> _logger;

    public CancelPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ILogger<CancelPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Payment> HandleAsync(
        CancelPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling cancel command for payment {PaymentId}", command.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(
            command.PaymentId,
            cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Cancel failed. Payment {PaymentId} was not found", command.PaymentId);
            throw new KeyNotFoundException(
                $"Payment with ID '{command.PaymentId}' was not found.");
        }

        payment.Cancel();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment {PaymentId} cancelled successfully", command.PaymentId);

        return payment;
    }
}
