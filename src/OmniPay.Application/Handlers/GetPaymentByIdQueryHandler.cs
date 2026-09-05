using Microsoft.Extensions.Logging;
using OmniPay.Application.Interfaces;
using OmniPay.Application.Queries;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Handlers;

public sealed class GetPaymentByIdQueryHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

    public GetPaymentByIdQueryHandler(
        IPaymentRepository paymentRepository,
        ILogger<GetPaymentByIdQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Payment?> HandleAsync(
        GetPaymentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment {PaymentId}", query.Id);

        var payment = await _paymentRepository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Payment {PaymentId} was not found", query.Id);
        }

        return payment;
    }
}
