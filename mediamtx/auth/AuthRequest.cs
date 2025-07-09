namespace auth
{
    public class AuthRequest
    {
        public string? Ip { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? Action { get; set; }
        public string? Path { get; set; }
        public string? Protocol { get; set; }
        public string? Id { get; set; }
        public string? Query { get; set; }
        public string? TraceId { get; set; }
    }
}
