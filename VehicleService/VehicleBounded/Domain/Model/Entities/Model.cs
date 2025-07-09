namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Model
{
    public int Id { get; set; }
    public string CarModel { get; set; } = null!;
    
    public int BrandId { get; set; }               // Foreign key
    public Brand Brand { get; set; } = null!;      // Navigation property

    public Model() { }

    public Model(string carModel, int brandId)
    {
        if (string.IsNullOrWhiteSpace(carModel))
            throw new ArgumentException("Model name cannot be empty.");

        CarModel = carModel;
        BrandId = brandId;
    }

    public void UpdateCarModel(string carModel)
    {
        if (string.IsNullOrWhiteSpace(carModel))
            throw new ArgumentException("Model name cannot be empty.");

        CarModel = carModel;
    }

    public void SetBrand(int brandId)
    {
        BrandId = brandId;
    }
}