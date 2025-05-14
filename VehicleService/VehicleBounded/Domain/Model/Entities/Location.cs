namespace VehicleService.VehicleBounded.Domain.Model.Entities;

public class Location
{
    public int Id { get; private set; }
    public string Address { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string LocationStatus { get; private set; } = null!;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    protected Location() { }

    public Location(string address, string city, string country, string status, decimal latitude, decimal longitude)
    {
        Address = address;
        City = city;
        Country = country;
        LocationStatus = status;
        Latitude = latitude;
        Longitude = longitude;
    }
    public void UpdateDetails(string address, string city, string country, string status, decimal latitude, decimal longitude)
    {
        Address = address;
        City = city;
        Country = country;
        LocationStatus = status;
        Latitude = latitude;
        Longitude = longitude;
    }

    
}