using System;

namespace OmniPay.Application.Commands;

public sealed record CancelPaymentCommand(Guid PaymentId);
