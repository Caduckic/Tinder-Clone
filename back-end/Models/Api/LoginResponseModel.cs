namespace back_end.Models.Api
{
    public struct TokenResponseModel
    {
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class LoginResponseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
