namespace CorePackages.Infrastructure.Dto
{
    public class CreateUserRequest
    {
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool emailVerified { get; set; }
        public bool enabled { get; set; }
        public Credential[] credentials { get; set; }
    }

    public class Credential
    {
        public bool temporary { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

}
