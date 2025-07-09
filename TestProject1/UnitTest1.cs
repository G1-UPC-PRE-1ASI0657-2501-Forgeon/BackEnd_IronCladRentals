using System;
using NUnit.Framework;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleTest;

public class CrearVehiculoTest
{
    [Test]
    public void CrearVehiculo_ConDatosValidos_RetornaVehiculoCorrecto()
    {
        // Arrange
        var passengers = 4;
        var luggageCapacity = 2;
        var licensePlate = "ABC123";
        var color = "Blanco";
        var year = 2022;
        var transmission = "Automática";
        var fuelType = "Gasolina";
        var imageUrl = "url_imagen.jpg";
        var modelId = 1;
        var brandId = 1;
        var companyId = 1;

        // Act
        var vehicle = new Vehicle(
            passengers,
            luggageCapacity,
            licensePlate,
            color,
            year,
            transmission,
            fuelType,
            imageUrl,
            modelId,
            brandId,
            companyId
        );

        // Assert
        Assert.IsNotNull(vehicle);
        Assert.AreEqual(passengers, vehicle.Passengers);
        Assert.AreEqual(luggageCapacity, vehicle.LuggageCapacity);
        Assert.AreEqual(licensePlate, vehicle.LicensePlate);
        Assert.AreEqual(color, vehicle.Color);
        Assert.AreEqual(year, vehicle.Year);
        Assert.AreEqual(transmission, vehicle.Transmission);
        Assert.AreEqual(fuelType, vehicle.FuelType);
        Assert.AreEqual(imageUrl, vehicle.ImageUrl);
        Assert.AreEqual(modelId, vehicle.ModelId);
        Assert.AreEqual(brandId, vehicle.BrandId);
        Assert.AreEqual(companyId, vehicle.CompanyId);
    }
}