namespace CorePackages.Infrastructure.Dto
{
    public class CachedToken
    {
        public string accessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
