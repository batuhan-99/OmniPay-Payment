using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniPay.Domain.Entities;

public class Merchant
{
    private readonly List<Payment> _payments = new();

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // Navigation property for EF Core
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    // Required by Entity Framework Core
    private Merchant()
    {
    }

    public Merchant(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Merchant name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Merchant email cannot be empty.",
                nameof(email));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}