namespace RentalService.RentalBounded.Domain.Model.Aggregates;

public class Rental
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int VehicleId { get; set; }
    public int LocationId { get; set; }
    public int CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public String RentalStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public bool Paid { get; set; }
    protected Rental() { }

    public Rental(Guid userId, int vehicleId,int companyId,int locationId, DateTime startDate, DateTime endDate, String rentalStatus,decimal totalPrice,bool paid)
    {
        UserId = userId;
        VehicleId = vehicleId;
        CompanyId = companyId;
        LocationId = locationId;
        StartDate = startDate;
        EndDate = endDate;
        RentalStatus = "Pending";
        TotalPrice = totalPrice;
        Paid = paid;
    }

    public void GoPaid() => Paid = true;

    public void Confirm() => RentalStatus = "Confirmed";
    public void Cancel() => RentalStatus = "Cancelled";
    public void Complete() => RentalStatus = "Completed";
    public void Extend(DateTime newEndDate)
    {
        if (newEndDate <= EndDate)
            throw new InvalidOperationException("La nueva fecha debe ser posterior a la actual.");

        EndDate = newEndDate;
    }
    public void UpdateTotalPrice(decimal newTotal)
    {
        if (newTotal < 0)
            throw new ArgumentException("El precio total no puede ser negativo.");
        
        TotalPrice = newTotal;
    }
    public void SetTotalPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("El precio total no puede ser negativo.");

        TotalPrice = price;
    }



}