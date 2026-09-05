using Microsoft.EntityFrameworkCore;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;
using OmniPay.Infrastructure.Persistence;

namespace OmniPay.Infrastructure.Repositories;

public sealed class EfCorePaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _dbContext;

    public EfCorePaymentRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(payment => payment.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        await _dbContext.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
