using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs
{
    public class AddressDTO
    {
        [Required]
        public Guid AddressId { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]

        public string City { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]

        public string State { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]

        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; } 

        public UserAddress ToUserAddress()
        {
            var defaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var address = new UserAddress
            {
                AddressId = AddressId == defaultGuid ? Guid.NewGuid() : AddressId,
                Street = Street,
                City = City,
                State = State,
                Country = Country,
                IsDefault = IsDefault
            };
            return address;
        }

    }
}
