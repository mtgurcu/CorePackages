using System.Text.Json;

namespace CorePackages.Infrastructure.Dto
{

    public class VaultLoginRequest
    {
        public string role_id { get; set; }
        public string secret_id { get; set; }
    }

    public class VaultLoginResponse
    {
        public string request_id { get; set; }
        public string lease_id { get; set; }
        public bool renewable { get; set; }
        public int lease_duration { get; set; }
        public object data { get; set; }
        public object wrap_info { get; set; }
        public object warnings { get; set; }
        public Auth auth { get; set; }
        public string mount_type { get; set; }
    }

    public class Auth
    {
        public string client_token { get; set; }
        public string accessor { get; set; }
        public string[] policies { get; set; }
        public string[] token_policies { get; set; }
        public int lease_duration { get; set; }
        public bool renewable { get; set; }
        public string entity_id { get; set; }
        public string token_type { get; set; }
        public bool orphan { get; set; }
        public object mfa_requirement { get; set; }
        public int num_uses { get; set; }
    }

    public class VaultSecretResponse
    {
        public string request_id { get; set; }

        public string lease_id { get; set; }

        public bool renewable { get; set; }

        public int lease_duration { get; set; }

        public VaultSecretResponseData data { get; set; }

        public object wrap_info { get; set; }

        public object warnings { get; set; }

        public object auth { get; set; }
    }

    public class VaultSecretResponseData
    {
        public JsonElement data { get; set; }
        public Metadata metadata { get; set; }
    }
    public class Metadata
    {
        public string created_time { get; set; }
        public object custom_metadata { get; set; }
        public string deletion_time { get; set; }
        public bool destroyed { get; set; }
        public int version { get; set; }
    }
    public class VaultConfig
    {
        public string RoleId { get; set; }
        public string SecretId { get; set; }
        public string Token { get; set; }
        public string Engine { get; set; }
        public string Address { get; set; }
        public bool? Enable { get; set; }
    }
}
