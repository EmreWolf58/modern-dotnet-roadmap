namespace TaskManagement.Api.Model
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
