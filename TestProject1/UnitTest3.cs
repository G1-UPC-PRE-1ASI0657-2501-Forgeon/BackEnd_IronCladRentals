using NUnit.Framework;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleTest;

public class ActualizarDetallesVehiculoTest
{
    [Test]
    public void ActualizarDetallesVehiculo_CambiaPropiedadesCorrectamente()
    {
        var vehicle = new Vehicle(4, 2, "ABC123", "Blanco", 2022, "Automática", "Gasolina", "img.jpg", 1, 1, 1);
        vehicle.UpdateVehicleDetails(5, 3, "XYZ789", "Negro", 2023, "Manual", "Diesel", "img2.jpg", 2, 2, 2);
        Assert.AreEqual(5, vehicle.Passengers);
        Assert.AreEqual("XYZ789", vehicle.LicensePlate);
        Assert.AreEqual("Negro", vehicle.Color);
    }
}