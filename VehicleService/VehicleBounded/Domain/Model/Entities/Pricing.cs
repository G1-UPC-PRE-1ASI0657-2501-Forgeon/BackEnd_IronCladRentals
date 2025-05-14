namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Pricing
{
    public int Id { get; private set; }

    public decimal DailyRate { get; private set; }
    public decimal WeeklyRate { get; private set; }
    public decimal Discount { get; private set; }

    public int VehicleId { get; private set; }

    protected Pricing() { }

    public Pricing(decimal dailyRate, decimal weeklyRate, decimal discount, int vehicleId)
    {
        DailyRate = dailyRate;
        WeeklyRate = weeklyRate;
        Discount = discount;
        VehicleId = vehicleId;
    }
    
    public void AssignToVehicle(int vehicleId)
    {
        VehicleId = vehicleId;
    }

    public void UpdateRates(decimal dailyRate, decimal weeklyRate, decimal discount)
    {
        DailyRate = dailyRate;
        WeeklyRate = weeklyRate;
        Discount = discount;
    }

}