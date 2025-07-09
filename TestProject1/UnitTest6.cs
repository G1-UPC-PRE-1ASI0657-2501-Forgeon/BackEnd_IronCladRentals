using NUnit.Framework;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using System;

namespace VehicleTest;

public class CrearRentalTest
{
    [Test]
    public void CrearRental_ConDatosValidos_RetornaRentalCorrecto()
    {
        var userId = Guid.NewGuid();
        var vehicleId = 1;
        var companyId = 1;
        var locationId = 1;
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(3);
        var rentalStatus = "Pending";
        var totalPrice = 100m;
        var paid = false; // o true, según lo que desees probar
        var rental = new Rental(userId, vehicleId, companyId, locationId, startDate, endDate, rentalStatus, totalPrice, paid);
        
        Assert.IsNotNull(rental);
        Assert.AreEqual(userId, rental.UserId);
        Assert.AreEqual(vehicleId, rental.VehicleId);
        Assert.AreEqual(companyId, rental.CompanyId);
        Assert.AreEqual(locationId, rental.LocationId);
        Assert.AreEqual(startDate, rental.StartDate);
        Assert.AreEqual(endDate, rental.EndDate);
        Assert.AreEqual("Pending", rental.RentalStatus);
        Assert.AreEqual(totalPrice, rental.TotalPrice);
    }
}