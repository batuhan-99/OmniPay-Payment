using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Application.DTOs;
using OmniPay.Application.Interfaces;
using OmniPay.Domain.Entities;

namespace OmniPay.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<Payment> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = new Payment(
            request.MerchantId,
            request.Amount,
            request.Currency,
            request.Description);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }

    public async Task<Payment?> GetPaymentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Payment> MarkAsSuccessfulAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetRequiredPaymentAsync(id, cancellationToken);

        payment.MarkAsSuccessful();
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }

    public async Task<Payment> MarkAsFailedAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetRequiredPaymentAsync(id, cancellationToken);

        payment.MarkAsFailed();
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }

    public async Task<Payment> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetRequiredPaymentAsync(id, cancellationToken);

        payment.Cancel();
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }

    public async Task<Payment> ProcessPaymentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetRequiredPaymentAsync(id, cancellationToken);

        var gatewayResult = await _paymentGateway.ProcessPaymentAsync(
            payment,
            cancellationToken);

        if (gatewayResult)
        {
            payment.MarkAsSuccessful();
        }
        else
        {
            payment.MarkAsFailed();
        }

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return payment;
    }

    private async Task<Payment> GetRequiredPaymentAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        if (payment is null)
        {
            throw new KeyNotFoundException($"Payment with ID '{id}' was not found.");
        }

        return payment;
    }
}