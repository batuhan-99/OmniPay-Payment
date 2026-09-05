using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Application.DTOs;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetPaymentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Payment> MarkAsSuccessfulAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Payment> MarkAsFailedAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Payment> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Payment> ProcessPaymentAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}