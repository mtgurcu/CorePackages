namespace CorePackages.Persistance.Entity
{
    public class OutboxEntity : BaseEntity
    {
        public string? HttpContextId { get; set; }
        public bool IsLocked { get; set; }
        public int RetryCount { get; set; }
        public string Status { get; set; }
        public string? Payload { get; set; }
        public string Type { get; set; }
    }
}
