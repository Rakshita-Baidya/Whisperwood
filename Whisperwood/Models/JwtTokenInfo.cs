namespace Whisperwood.Models
{
    public class JwtTokenInfo
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int ExpiryInMinutes { get; set; }
        public required string Key { get; set; }
    }
}
