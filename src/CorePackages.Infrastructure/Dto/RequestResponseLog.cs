namespace CorePackages.Infrastructure.Dto
{
    public class RequestResponseLog : ILoggableAsInformation
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string RequestHeaders { get; set; }
        public string Request { get; set; }
        public string ResponseHeaders { get; set; }
        public string Response { get; set; }
        public int StatusCode { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public DateTime Timestamp { get; set; }
        public string HttpContextId { get; set; }
    }

    public interface ILoggableAsInformation { }
}
