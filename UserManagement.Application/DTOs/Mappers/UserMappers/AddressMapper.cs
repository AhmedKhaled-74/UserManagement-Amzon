using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs.Mappers.UserMappers
{
    public static class AddressMapper
    {
        public static AddressDTO ToUserAddressDTO(this UserAddress userAddress)
        {
            var address = new AddressDTO
            {
                AddressId = userAddress.AddressId,
                Street = userAddress.Street,
                City = userAddress.City,
                State = userAddress.State,
                Country = userAddress.Country,
                IsDefault = userAddress.IsDefault
            };
            return address;
        }
    }

    }
