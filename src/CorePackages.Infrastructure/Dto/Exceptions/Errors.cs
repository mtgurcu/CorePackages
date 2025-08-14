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
            public static readonly Error UserCreateError = new("U001", "User can not created.");
            public static readonly Error UserUpdateError = new("U002", "User can not updated.");
            public static readonly Error UserNotExist = new("U003", "User not exist.");
        }
        public static class Validation
        {
            public static Error ValidationErrors(string errors) => new Error("V001", $"{errors}");
        }
    }
}
