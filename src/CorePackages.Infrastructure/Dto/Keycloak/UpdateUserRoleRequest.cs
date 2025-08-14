using System.Text.Json.Serialization;

namespace CorePackages.Infrastructure.Dto.Keycloak
{
    public class UpdateUserRoleRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
