namespace GDriveApi.Model
{
    public class AuthResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
    }
}