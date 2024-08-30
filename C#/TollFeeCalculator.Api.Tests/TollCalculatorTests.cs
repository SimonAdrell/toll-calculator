using Moq;
using TollFeeCalculator.Api.Services;
using TollFeeCalculator.Api.Vehicles;

namespace TollFeeCalculator.Api.Tests;

public class TollCalculatorTests
{
    [Fact]
    public void GetTollFee_Returns_CorrectFee()
    {
        // Arrange
        var vehicle = new Car();

        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var passes = new DateTime[]{
                DateTime.Parse("2024-08-29 08:00:00")
                        };

        // Act 
        var fee = sut.GetTollFee(vehicle, passes);

        // Assert;
        Assert.Equal(13, fee);
    }


    [Fact]
    public void GetTollFee_WithingHour_RendersHighestFee()
    {
        // Arrange
        var vehicle = new Car();
        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var passes = new DateTime[]{
                DateTime.Parse("2024-08-29 06:30:00"),
                DateTime.Parse("2024-08-29 07:20:00")
                        };

        // Act 
        var fee = sut.GetTollFee(vehicle, passes);

        // Assert;
        Assert.Equal(18, fee);
    }

    [Fact]
    public void GetTollFee_Multiple_RendersMaxTotalFee()
    {
        // Arrange
        var vehicle = new Car();

        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var passes = new DateTime[]{
                DateTime.Parse("2024-08-29 07:20:00"),
                DateTime.Parse("2024-08-29 07:20:00"),
                DateTime.Parse("2024-08-29 07:20:00"),
                DateTime.Parse("2024-08-29 07:20:00"),
                        };

        // Act 
        var fee = sut.GetTollFee(vehicle, passes);

        // Assert

        var MAX_TOTAL_FEE = 60;

        Assert.Equal(MAX_TOTAL_FEE, fee);
    }


    [Theory]
    [InlineData("07:30:00")]
    [InlineData("16:00:00")]
    public void GetTollFee_RushHour_RendersMaxHourlyFee(string TimeOfDay)
    {
        // Arrange
        var vehicle = new Car();
        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);

        var timeOnly = TimeOnly.Parse(TimeOfDay);
        var dateOnly = new DateOnly(2024, 08, 29);

        // Act 
        var fee = sut.GetTollFee(vehicle, [dateOnly.ToDateTime(timeOnly)]);

        // Assert
        var MAX_HOURLY_FEE = 18;

        Assert.Equal(MAX_HOURLY_FEE, fee);
    }

    [Fact]
    public void GetTollFee_Weekend_NoFee()
    {
        // Arrange
        var vehicle = new Car();
        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var passes = new DateTime[]{
                DateTime.Parse("2024-08-31 07:00:00")
                        };

        // Act 
        var fee = sut.GetTollFee(vehicle, passes);

        // Assert;
        Assert.Equal(0, fee);
    }

    [Fact]
    public void GetTollFee_TollFreeVehicle_ReturnsZero()
    {
        // Arrange
        var vehicle = new Motorbike();
        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var passes = new DateTime[]{
                DateTime.Parse("2024-08-29 08:00:00"),
                DateTime.Parse("2024-08-29 09:00:00")
                        };

        // Act 
        var fee = sut.GetTollFee(vehicle, passes);

        // Assert;
        Assert.Equal(0, fee);
    }

    [Theory]
    [InlineData("07:00", 18)]
    [InlineData("15:15", 13)]
    [InlineData("15:31", 18)]
    public void GetTollFee_Time_CorrectSum(string Time, double expectedSum)
    {
        // Arrange
        var mockedHolidayService = new Mock<IHolidayService>();
        mockedHolidayService.Setup(e => e.IsDateHoliday(It.IsAny<DateTime>())).Returns(false);
        var sut = new TollCalculator(mockedHolidayService.Object);
        var vehicle = new Car();
        var timeOnly = TimeOnly.Parse(Time);
        var dateOnly = new DateOnly(2024, 08, 29);

        // Act
        var sum = sut.GetTollFee(vehicle, [dateOnly.ToDateTime(timeOnly)]);

        // Assert
        Assert.Equal(expectedSum, sum);
    }
}
