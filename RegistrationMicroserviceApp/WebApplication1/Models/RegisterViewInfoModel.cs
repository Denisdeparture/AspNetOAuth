namespace WebClient.Models
{
    public class RegisterViewInfoModel
    {
        public string Name { get; set; } = null!;
        public string? LastName { get; set; }
        public string Mail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
