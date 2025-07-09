using NUnit.Framework;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleTest;

public class AsignarPrecioVehiculoTest
{
    [Test]
    public void AsignarPrecioVehiculo_ValoresCorrectos()
    {
        var vehicle = new Vehicle(4, 2, "ABC123", "Blanco", 2022, "Automática", "Gasolina", "img.jpg", 1, 1, 1);
        vehicle.SetPricing(100m, 600m, 10m);
        Assert.IsNotNull(vehicle.Pricing);
        Assert.AreEqual(100m, vehicle.Pricing.DailyRate);
    }
}