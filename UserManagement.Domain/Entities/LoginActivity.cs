using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities
{
    public class LoginActivity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string IpAddress { get; set; } = "";
        public string Attempt { get; set; } = LoginAttempts.Success.ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
   

}
