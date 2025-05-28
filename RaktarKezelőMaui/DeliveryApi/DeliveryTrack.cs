namespace DeliveryApi
{
    public class DeliveryTrack
    {
            public Guid PackageId { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string Location { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }
}

