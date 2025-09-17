using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Domain.LogsEntities
{
    public class RoleActivity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }


    }

}
