using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Domain.Enums;

namespace OmniPay.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }

    public Guid MerchantId { get; private set; }

    public Merchant Merchant { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public PaymentStatus Status { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // Required by Entity Framework Core
    private Payment()
    {
    }

    public Payment(
        Guid merchantId,
        decimal amount,
        string currency,
        string? description = null)
    {
        if (merchantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Merchant ID cannot be empty.",
                nameof(merchantId));
        }

        if (amount <= 0)
        {
            throw new ArgumentException(
                "Payment amount must be greater than zero.",
                nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException(
                "Currency cannot be empty.",
                nameof(currency));
        }

        Id = Guid.NewGuid();
        MerchantId = merchantId;
        Amount = amount;
        Currency = currency.ToUpperInvariant();
        Description = description;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsSuccessful()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payments can be marked as successful.");
        }

        Status = PaymentStatus.Successful;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payments can be marked as failed.");
        }

        Status = PaymentStatus.Failed;
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payments can be cancelled.");
        }

        Status = PaymentStatus.Cancelled;
    }
}