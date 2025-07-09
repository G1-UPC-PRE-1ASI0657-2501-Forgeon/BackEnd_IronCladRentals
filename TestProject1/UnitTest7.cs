using NUnit.Framework;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using System;

namespace VehicleTest;

public class CambiarEstadoRentalTest
{
    [Test]
    public void CambiarEstadoRental_ActualizaStatus()
    {
        var userId = Guid.NewGuid();
        var vehicleId = 1;
        var companyId = 1;
        var locationId = 1;
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(2);
        var rentalStatus = "Pendiente";
        var totalPrice = 200m;
        var paid = false;

        var rental = new Rental(userId, vehicleId, companyId, locationId, startDate, endDate, rentalStatus, totalPrice, paid);
        rental.RentalStatus = "Confirmado";
        Assert.AreEqual("Confirmado", rental.RentalStatus);
    }
}