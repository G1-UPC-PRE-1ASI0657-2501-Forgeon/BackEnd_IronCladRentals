using NUnit.Framework;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleTest;

public class CambiarEmpresaVehiculoTest
{
    [Test]
    public void CambiarEmpresaVehiculo_ActualizaCompanyId()
    {
        var vehicle = new Vehicle(4, 2, "ABC123", "Blanco", 2022, "Automática", "Gasolina", "img.jpg", 1, 1, 1);
        vehicle.SetCompany(5);
        Assert.AreEqual(5, vehicle.CompanyId);
    }
}