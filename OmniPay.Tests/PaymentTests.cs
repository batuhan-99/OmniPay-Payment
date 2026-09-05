using OmniPay.Domain.Entities;
using OmniPay.Domain.Enums;
using Xunit;

namespace OmniPay.Tests;

public class PaymentTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePaymentWithPendingStatus()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var amount = 100m;
        var currency = "TRY";
        var description = "Test Payment";

        // Act
        var payment = new Payment(merchantId, amount, currency, description);

        // Assert
        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(merchantId, payment.MerchantId);
        Assert.Equal(amount, payment.Amount);
        Assert.Equal("TRY", payment.Currency);
        Assert.Equal(description, payment.Description);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Constructor_WithZeroOrNegativeAmount_ShouldThrowArgumentException(decimal invalidAmount)
    {
        // Arrange
        var merchantId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Payment(merchantId, invalidAmount, "TRY", "Invalid Amount Test"));
    }

    [Fact]
    public void MarkAsSuccessful_WhenPaymentIsPending_ShouldUpdateStatusToSuccessful()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 250m, "TRY");

        // Act
        payment.MarkAsSuccessful();

        // Assert
        Assert.Equal(PaymentStatus.Successful, payment.Status);
    }

    [Fact]
    public void Cancel_WhenPaymentIsAlreadySuccessful_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 250m, "TRY");
        payment.MarkAsSuccessful();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.Cancel());
    }
}