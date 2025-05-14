using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Model.Aggregates;

public class Vehicle
{
    public int Id { get; private set; }
    public int Passengers { get; private set; }
    public int LuggageCapacity { get; private set; }

    public int ModelId { get; private set; }
    public Entities.Model Model { get; private set; }

    public int BrandId { get; private set; }
    public Brand Brand { get; private set; }

    public int CompanyId { get; private set; }
    public Company Company { get; private set; }

    public Pricing Pricing { get; private set; }


    protected Vehicle() { }

    public Vehicle(int passengers, int luggageCapacity, int modelId, int brandId)
    {
        Passengers = passengers;
        LuggageCapacity = luggageCapacity;
        ModelId = modelId;
        BrandId = brandId;
    }

    public void SetPricing(decimal dailyRate, decimal weeklyRate, decimal discount)
    {
        Pricing = new Pricing(dailyRate, weeklyRate, discount, this.Id);
    }

   
    public void UpdateVehicleDetails(int passengers, int luggage, int modelId, int brandId, int companyId)
    {
        // Aquí puedes aplicar validaciones de dominio si es necesario
        Passengers = passengers;
        LuggageCapacity = luggage;
        ModelId = modelId;
        BrandId = brandId;
        CompanyId = companyId;
    }
    public void SetCompany(int companyId)
    {
        CompanyId = companyId;
    }

}