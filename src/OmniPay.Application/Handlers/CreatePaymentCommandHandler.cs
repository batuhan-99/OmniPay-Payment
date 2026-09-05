using Microsoft.Extensions.Logging;
using OmniPay.Application.Commands;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class CreatePaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ILogger<CreatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Payment> HandleAsync(
        CreatePaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating payment for merchant {MerchantId} with amount {Amount} {Currency}",
            command.MerchantId,
            command.Amount,
            command.Currency);

        var payment = new Payment(
            command.MerchantId,
            command.Amount,
            command.Currency,
            command.Description);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment {PaymentId} created successfully", payment.Id);

        return payment;
    }
}
