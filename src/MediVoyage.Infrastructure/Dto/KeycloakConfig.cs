namespace CorePackages.Infrastructure.Dto
{
    public class KeycloakConfig
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string Authority { get; set; }
        public string Audience { get; set; }
        public string RealmName { get; set; }
        public List<string> Scopes { get; set; }
    }
}
