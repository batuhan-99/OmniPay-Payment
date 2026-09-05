using Microsoft.Extensions.Logging;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Infrastructure.Gateways;

public sealed class SimulatedPaymentGateway : IPaymentGateway
{
    private readonly ILogger<SimulatedPaymentGateway> _logger;

    public SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger)
    {
        _logger = logger;
    }

    public Task<bool> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        _logger.LogInformation(
            "Simulated payment gateway processing payment {PaymentId}",
            payment.Id);

        // Sandbox/simulated behavior: always return success.
        return Task.FromResult(true);
    }
}
