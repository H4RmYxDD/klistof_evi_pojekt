using System;

public class Delivery
{
    public int Id { get; set; }
    public Guid PackageId { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string Location { get; set; }
    public string? Notes { get; set; }
}