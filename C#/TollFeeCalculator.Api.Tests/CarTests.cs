using TollFeeCalculator.Api.Vehicles;

namespace TollFeeCalculator.Api.Tests
{
    public class CarTests
    {
        [Fact]
        public void GetVehicleType_ReturnsCar()
        {
            // Arrange

            var car = new Car();
            
            // Act 
            var vehicleType = car.GetVehicleType();

            // Assert;
            Assert.Equal("Car",vehicleType);

        }
    }
}