using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs.Mappers.UserMappers
{
    public static class PhoneMapper
    {
        public static PhoneDTO ToUserPhoneDTO(this UserPhone userPhone)
        {
            var phone = new PhoneDTO
            {
                PhoneId = userPhone.PhoneId,
                PhoneNumber = userPhone.PhoneNumber,
                IsDefault = userPhone.IsDefault
            };
            return phone;
        }
    }

    }
