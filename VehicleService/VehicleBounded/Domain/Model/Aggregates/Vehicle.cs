using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Model.Aggregates;

public class Vehicle
{
    public int Id { get; set; }
    public int Passengers { get; set; }
    public int LuggageCapacity { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public Entities.Model Model { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public Pricing Pricing { get; set; }


    protected Vehicle() { }

    // Constructor público para casos específicos (como cache)
    public Vehicle(int id, int passengers, int luggageCapacity, string licensePlate, 
                   string color, int year, string transmission, string fuelType, 
                   string imageUrl, int modelId, int brandId, int companyId)
    {
        Id = id;
        Passengers = passengers;
        LuggageCapacity = luggageCapacity;
        LicensePlate = licensePlate;
        Color = color;
        Year = year;
        Transmission = transmission;
        FuelType = fuelType;
        ImageUrl = imageUrl;
        ModelId = modelId;
        BrandId = brandId;
        CompanyId = companyId;
    }

    public Vehicle(
        int passengers,
        int luggageCapacity,
        string licensePlate,
        string color,
        int year,
        string transmission,
        string fuelType,
        string imageUrl,

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
        ModelId = modelId;
        BrandId = brandId;
        CompanyId = companyId;
    }

    public void SetCompany(int companyId)
    {
        CompanyId = companyId;
    }

}