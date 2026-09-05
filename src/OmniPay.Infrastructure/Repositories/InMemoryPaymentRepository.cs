using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Infrastructure.Repositories;

public sealed class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = new();
    private readonly object _lock = new();

    public Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var payment = _payments.FirstOrDefault(payment => payment.Id == id);

            return Task.FromResult(payment);
        }
    }

    public Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        lock (_lock)
        {
            _payments.Add(payment);
        }

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // In-memory storage does not require an explicit save operation.
        return Task.CompletedTask;
    }
}