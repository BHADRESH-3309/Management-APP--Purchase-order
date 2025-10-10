namespace WebManagementApp.Models
{
    public class ResponseModel
    {
        public bool IsError { get; set; }
        public IEnumerable<dynamic>? Result { get; set; }
        public IEnumerable<dynamic>? BrandResult { get; set; }
        public IEnumerable<dynamic>? ChannelResult { get; set; }
        public string? Message { get; set; }
        public string? Id { get; set; }
    }
}
