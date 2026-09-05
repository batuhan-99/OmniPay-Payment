using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniPay.Application.Commands;

public sealed record MarkPaymentAsSuccessfulCommand(Guid PaymentId);