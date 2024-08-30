namespace TollFeeCalculator.Api.Vehicles;

public class Car : IVehicle
{
    public string GetVehicleType()
    {
        return "Car";
    }
}