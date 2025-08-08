namespace CorePackages.Infrastructure.Dto
{
    public enum OutboxStatus
    {
        Initial = 0,
        Success = 1,
        Failed = 2
    }
    public enum JobType
    {
        ApiRequest = 1,
        Producer = 2,
        Unlock = 3
    }
}
