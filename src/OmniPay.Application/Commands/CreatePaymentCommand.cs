using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Application.DTOs;

namespace OmniPay.Application.Commands;

public sealed record CreatePaymentCommand(
    Guid MerchantId,
    decimal Amount,
    string Currency,
    string? Description)
{
    public CreatePaymentRequest ToRequest()
    {
        return new CreatePaymentRequest
        {
            MerchantId = MerchantId,
            Amount = Amount,
            Currency = Currency,
            Description = Description
        };
    }
}