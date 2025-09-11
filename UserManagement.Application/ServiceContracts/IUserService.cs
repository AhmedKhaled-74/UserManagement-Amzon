using System;
using System.Threading.Tasks;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.ServiceContracts
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByEmailAsync(string? email);
        Task<UserDTO?> UpdatePersonalInfoAsync(Guid? userId, UpdatePersonalInfoRequest request);
        Task<ICollection<AddressDTO>?> GetUserAddressesAsync(Guid? userId);
        Task AddAddressAsync(Guid? userId, AddressDTO? address);
        Task UpdateAddressAsync(Guid? userId, AddressDTO? address);
        Task DeleteAddressAsync(Guid? userId, Guid? addressId);
        Task SetAddressDefaultAsync(Guid? userId, Guid? addressId);
        Task<ICollection<PhoneDTO>?> GetUserPhonesAsync(Guid? userId);
        Task AddPhoneAsync(Guid? userId, PhoneDTO? phone);
        Task UpdatePhoneAsync(Guid? userId, PhoneDTO? phoneNumber);
        Task DeletePhoneAsync(Guid? userId, Guid? phoneId);
        Task SetPhoneDefaultAsync(Guid? userId, Guid? phoneId);


    }
}
