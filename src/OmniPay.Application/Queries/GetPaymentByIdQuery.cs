using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniPay.Application.Queries;

public sealed record GetPaymentByIdQuery(Guid Id);
