using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class UserActivity
    {   
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public string Action { get; set; } = null!;
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;   
        
    }
   
}
