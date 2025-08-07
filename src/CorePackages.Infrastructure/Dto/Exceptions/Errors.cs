namespace CorePackages.Infrastructure.Dto.Exceptions
{
    public readonly struct Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
    public static class Errors
    {
        public static class User
        {
            public static readonly Error UserCreateError = new("P006", "User can not created.");
            public static readonly Error UserUpdateError = new("P007", "User can not updated.");
        }
        public static class Validation
        {
            public static Error ValidationErrors(string errors) => new Error("V001", $"{errors}");
        }
    }
}
