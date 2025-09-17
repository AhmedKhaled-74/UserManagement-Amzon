namespace UserManagement.Application.DTOs.LogDTOs
{
    public class RoleActivityDTO
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
