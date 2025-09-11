using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs
{
    public class PhoneDTO
    {
        [Required]
        public Guid PhoneId { get; set; }
        [Required]
        [StringLength(21)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
        public UserPhone ToUserPhone()
        {
            var defaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var phone = new UserPhone
            {
                PhoneId = PhoneId == defaultGuid ? Guid.NewGuid() : PhoneId,
                PhoneNumber = PhoneNumber,
                IsDefault = IsDefault
            };
            return phone;
        }
    }
}
