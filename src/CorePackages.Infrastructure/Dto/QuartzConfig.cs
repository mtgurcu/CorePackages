namespace CorePackages.Infrastructure.Dto
{
    public class QuartzConfig
    {
        public List<QuartzConfigurations> Configurations { get; set; }
    }
    public class QuartzConfigurations
    {
        public string JobKey { get; set; }
        public int JobInterval { get; set; }
        public int MaxRetryCount { get; set; }
        public int BatchSize { get; set; }
    }
}
