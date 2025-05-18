namespace Models;

public class CourierSearchViewModel
{
    public Guid OrderId { get; set; }
    public string DepartureAddress { get; set; }
    public string DestinationAddress { get; set; }
    public double DistanceKm { get; set; }
    public decimal Cost { get; set; } 
}