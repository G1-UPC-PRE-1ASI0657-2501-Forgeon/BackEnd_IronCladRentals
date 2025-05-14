namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Brand
{
    public int Id { get; private set; }
    public string BrandName { get; private set; } = null!;
    
    public ICollection<Model> Models { get; private set; } = new List<Model>();


    protected Brand() { }

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