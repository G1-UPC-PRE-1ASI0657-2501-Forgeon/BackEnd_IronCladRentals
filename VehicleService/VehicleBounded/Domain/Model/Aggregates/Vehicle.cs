using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Model.Aggregates;

public class Vehicle
{
    public int Id { get; private set; }
    public int Passengers { get; private set; }
    public int LuggageCapacity { get; private set; }
    public string LicensePlate { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Transmission { get; private set; } = string.Empty;
    public string FuelType { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public string Address { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public int ModelId { get; private set; }
    public Entities.Model Model { get; private set; }

    public int BrandId { get; private set; }
    public Brand Brand { get; private set; }

    public int CompanyId { get; private set; }
    public Company Company { get; private set; }

    public Pricing Pricing { get; private set; }


    protected Vehicle() { }

    public Vehicle(
        int passengers,
        int luggageCapacity,
        string licensePlate,
        string color,
        int year,
        string transmission,
        string fuelType,
        string imageUrl,
        string address,
        string city,
        string country,
        decimal latitude,
        decimal longitude,
        int modelId,
        int brandId,
        int companyId)
    {
        Passengers = passengers;
        LuggageCapacity = luggageCapacity;
        LicensePlate = licensePlate;
        Color = color;
        Year = year;
        Transmission = transmission;
        FuelType = fuelType;
        ImageUrl = imageUrl;
        Address = address;
        City = city;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
        ModelId = modelId;
        BrandId = brandId;
        CompanyId = companyId;
    }

    public void SetPricing(decimal dailyRate, decimal weeklyRate, decimal discount)
    {
        Pricing = new Pricing(dailyRate, weeklyRate, discount, this.Id);
    }

   
    public void UpdateVehicleDetails(
        int passengers,
        int luggage,
        string licensePlate,
        string color,
        int year,
        string transmission,
        string fuelType,
        string imageUrl,
        string address,
        string city,
        string country,
        decimal latitude,
        decimal longitude,
        int modelId,
        int brandId,
        int companyId)
    {
        Passengers = passengers;
        LuggageCapacity = luggage;
        LicensePlate = licensePlate;
        Color = color;
        Year = year;
        Transmission = transmission;
        FuelType = fuelType;
        ImageUrl = imageUrl;
        Address = address;
        City = city;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
        ModelId = modelId;
        BrandId = brandId;
        CompanyId = companyId;
    }

    public void SetCompany(int companyId)
    {
        CompanyId = companyId;
    }

}