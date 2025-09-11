using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class UserPhone
    {


        public Guid PhoneId { get; set; }
        public Guid UserId { get; set; } // Foreign key to the user
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;

    }
}
