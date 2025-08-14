namespace CorePackages.Infrastructure.Dto.Keycloak
{
    public class RoleRepresantationRequest
    {
        public string Username { get; set; } = null!;
        public List<string> RolesToAdd { get; set; } = new();
        public List<string> RolesToRemove { get; set; } = new();
    }
    public class RoleRepresantationResponse
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Attributes
    {
    }

}
