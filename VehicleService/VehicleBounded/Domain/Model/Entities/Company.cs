namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string RUC { get; set; }
    
    public Guid UserId { get; set; }
    public List<Location> Locations { get; private set; } = new();

    public Company() { }

    protected Company(string name, Guid authUserId)
    {
        Name = name;
        UserId = authUserId;
    }
    public Company(string name, string ruc)
    {
        Name = name;
        RUC = ruc;
    }
    public void UpdateDetails(string newName, string newRuc)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Company name cannot be empty.");

        Name = newName;
        RUC = newRuc;
    }
    
    public void SetAuthUserId(Guid userId)
    {
        UserId = userId;
    }
}