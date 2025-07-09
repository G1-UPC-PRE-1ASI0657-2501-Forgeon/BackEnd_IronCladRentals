namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Brand
{
    public int Id { get; set; }
    public string BrandName { get; set; } = null!;
    
    public ICollection<Model> Models { get; private set; } = new List<Model>();

    public Brand() { }

    public Brand(string brandName)
    {
        BrandName = brandName;
    }
    
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Brand name cannot be empty.");

        BrandName = newName;
    }

}