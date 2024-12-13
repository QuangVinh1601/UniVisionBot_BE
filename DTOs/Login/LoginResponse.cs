namespace UniVisionBot.DTOs.Login
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } 

        //public string RefreshToken { get; set; }
        public string RoleUser { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
