using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> UpdatePersonalInfoAsync(Guid userId, UpdatePersonalInfoRequest request);
        Task<ICollection<UserAddress>?> GetUserAddressesAsync(Guid userId);
        Task AddAddressAsync(Guid userId, UserAddress address);
        Task UpdateAddressAsync(Guid addressId, UserAddress address);
        Task DeleteAddressAsync(Guid userId, Guid addressId);
        Task SetAddressDefaultAsync(Guid userId, Guid addressId);

        Task<ICollection<UserPhone>?> GetUserPhonesAsync(Guid userId);
        Task AddPhoneAsync(Guid userId, UserPhone phone);
        Task UpdatePhoneAsync(Guid userId, UserPhone phone);
        Task DeletePhoneAsync(Guid userId, Guid phoneId);
        Task SetPhoneDefaultAsync(Guid userId, Guid phoneId);
    }
}
