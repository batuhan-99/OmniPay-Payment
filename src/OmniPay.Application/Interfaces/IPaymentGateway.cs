using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OmniPay.Domain.Entities;

namespace OmniPay.Application.Interfaces;

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default);
}