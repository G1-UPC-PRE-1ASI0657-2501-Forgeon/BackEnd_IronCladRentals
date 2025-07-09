using NUnit.Framework;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using System;

namespace VehicleTest;

public class CalcularDuracionRentalTest
{
    [Test]
    public void CalcularDuracionRental_DiasCorrectos()
    {
        var userId = Guid.NewGuid();
        var vehicleId = 1;
        var companyId = 1;
        var locationId = 1;
        var startDate = new DateTime(2024, 6, 1);
        var endDate = new DateTime(2024, 6, 5);
        var rentalStatus = "Pendiente";
        var totalPrice = 400m;
        var paid = false;

        var rental = new Rental(userId, vehicleId, companyId, locationId, startDate, endDate, rentalStatus, totalPrice, paid);
        int dias = (rental.EndDate - rental.StartDate).Days;
        Assert.AreEqual(4, dias);
    }
}