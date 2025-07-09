using NUnit.Framework;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using System;

namespace VehicleTest;

public class ValidarAnioVehiculoTest
{
    [Test]
    public void CrearVehiculo_ConAnioFuturo_SeAsignaCorrectamente()
    {
        var anioFuturo = DateTime.Now.Year + 2;
        var vehicle = new Vehicle(4, 2, "XYZ789", "Negro", anioFuturo, "Manual", "Eléctrico", "img2.jpg", 1, 1, 1);
        Assert.AreEqual(anioFuturo, vehicle.Year);
    }
}