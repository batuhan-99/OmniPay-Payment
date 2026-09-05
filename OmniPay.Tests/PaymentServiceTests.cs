using Microsoft.Extensions.Logging.Abstractions;
using OmniPay.Application.Commands;
using OmniPay.Application.Handlers;
using OmniPay.Application.Interfaces;
using OmniPay.Application.Queries;
using OmniPay.Domain.Entities;
using OmniPay.Domain.Enums;

namespace OmniPay.Tests;

public class PaymentHandlerTests
{
    [Fact]
    public async Task CreatePaymentHandler_ShouldCreatePendingPayment()
    {
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentCommandHandler(repository, NullLogger<CreatePaymentCommandHandler>.Instance);

        var command = new CreatePaymentCommand(Guid.NewGuid(), 100m, "TRY", "Test");

        var payment = await handler.HandleAsync(command);

        Assert.NotNull(payment);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Single(repository.Payments);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task GetPaymentByIdQueryHandler_ShouldReturnExistingPayment()
    {
        var repository = new FakePaymentRepository();
        var existingPayment = new Payment(Guid.NewGuid(), 125m, "TRY");
        await repository.AddAsync(existingPayment);

        var handler = new GetPaymentByIdQueryHandler(repository, NullLogger<GetPaymentByIdQueryHandler>.Instance);

        var payment = await handler.HandleAsync(new GetPaymentByIdQuery(existingPayment.Id));

        Assert.NotNull(payment);
        Assert.Equal(existingPayment.Id, payment!.Id);
    }

    [Fact]
    public async Task GetPaymentByIdQueryHandler_ShouldReturnNull_WhenPaymentDoesNotExist()
    {
        var repository = new FakePaymentRepository();
        var handler = new GetPaymentByIdQueryHandler(repository, NullLogger<GetPaymentByIdQueryHandler>.Instance);

        var payment = await handler.HandleAsync(new GetPaymentByIdQuery(Guid.NewGuid()));

        Assert.Null(payment);
    }

    [Fact]
    public async Task ProcessPaymentHandler_ShouldMarkSuccessful_WhenGatewayReturnsTrue()
    {
        var repository = new FakePaymentRepository();
        var payment = new Payment(Guid.NewGuid(), 250m, "TRY");
        await repository.AddAsync(payment);

        var gateway = new FakePaymentGateway(true);
        var handler = new ProcessPaymentCommandHandler(
            repository,
            gateway,
            NullLogger<ProcessPaymentCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ProcessPaymentCommand(payment.Id));

        Assert.Equal(PaymentStatus.Successful, result.Status);
        Assert.True(gateway.WasCalled);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task ProcessPaymentHandler_ShouldMarkFailed_WhenGatewayReturnsFalse()
    {
        var repository = new FakePaymentRepository();
        var payment = new Payment(Guid.NewGuid(), 250m, "TRY");
        await repository.AddAsync(payment);

        var gateway = new FakePaymentGateway(false);
        var handler = new ProcessPaymentCommandHandler(
            repository,
            gateway,
            NullLogger<ProcessPaymentCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ProcessPaymentCommand(payment.Id));

        Assert.Equal(PaymentStatus.Failed, result.Status);
        Assert.True(gateway.WasCalled);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task MarkPaymentAsSuccessfulHandler_ShouldMarkPaymentAsSuccessful()
    {
        var repository = new FakePaymentRepository();
        var payment = new Payment(Guid.NewGuid(), 80m, "TRY");
        await repository.AddAsync(payment);

        var handler = new MarkPaymentAsSuccessfulCommandHandler(
            repository,
            NullLogger<MarkPaymentAsSuccessfulCommandHandler>.Instance);

        var result = await handler.HandleAsync(new MarkPaymentAsSuccessfulCommand(payment.Id));

        Assert.Equal(PaymentStatus.Successful, result.Status);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task MarkPaymentAsFailedHandler_ShouldMarkPaymentAsFailed()
    {
        var repository = new FakePaymentRepository();
        var payment = new Payment(Guid.NewGuid(), 80m, "TRY");
        await repository.AddAsync(payment);

        var handler = new MarkPaymentAsFailedCommandHandler(
            repository,
            NullLogger<MarkPaymentAsFailedCommandHandler>.Instance);

        var result = await handler.HandleAsync(new MarkPaymentAsFailedCommand(payment.Id));

        Assert.Equal(PaymentStatus.Failed, result.Status);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task CancelPaymentHandler_ShouldCancelPendingPayment()
    {
        var repository = new FakePaymentRepository();
        var payment = new Payment(Guid.NewGuid(), 50m, "TRY");
        await repository.AddAsync(payment);

        var handler = new CancelPaymentCommandHandler(
            repository,
            NullLogger<CancelPaymentCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CancelPaymentCommand(payment.Id));

        Assert.Equal(PaymentStatus.Cancelled, result.Status);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task CancelPaymentHandler_ShouldThrow_WhenPaymentDoesNotExist()
    {
        var repository = new FakePaymentRepository();

        var handler = new CancelPaymentCommandHandler(
            repository,
            NullLogger<CancelPaymentCommandHandler>.Instance);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.HandleAsync(new CancelPaymentCommand(Guid.NewGuid())));
    }

    private sealed class FakePaymentRepository : IPaymentRepository
    {
        public List<Payment> Payments { get; } = new();

        public bool SaveChangesCalled { get; private set; }

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Payments.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            Payments.Add(payment);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakePaymentGateway : IPaymentGateway
    {
        private readonly bool _result;

        public bool WasCalled { get; private set; }

        public FakePaymentGateway(bool result)
        {
            _result = result;
        }

        public Task<bool> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(_result);
        }
    }
}
