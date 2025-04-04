namespace Security.JWT.DTOs
{
    public class TokenPair
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string RefreshHasherToken { get; set; }
    }
}
