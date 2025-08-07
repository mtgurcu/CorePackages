namespace CorePackages.Infrastructure.Dto.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }
        public BusinessException(Error error)
        : base(error.Message)
        {
            ErrorCode = error.Code;
        }
    }
}
