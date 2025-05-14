namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Model
{
    public int Id { get; private set; }
    public string CarModel { get; private set; } = null!;

    protected Model() { }

    public Model(string carModel)
    {
        CarModel = carModel;
    }
    
    public void UpdateCarModel(string carModel)
    {
        if (string.IsNullOrWhiteSpace(carModel))
            throw new ArgumentException("Model name cannot be empty.");

        CarModel = carModel;
    }

}