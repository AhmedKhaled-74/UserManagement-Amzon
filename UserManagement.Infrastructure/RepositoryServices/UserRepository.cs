using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Mappers.AdminMappers;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Application.ServiceContracts;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.Identity;
using UserManagement.Infrastructure.DbContexts;

namespace UserManagement.Infrastructure.RepositoryServices
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserPublisher _userPublisher;
        private readonly AppDbContext _db;

        public UserRepository(UserManager<ApplicationUser> userManager, IUserPublisher userPublisher, AppDbContext db)
        {
            _userManager = userManager;
            _userPublisher = userPublisher;
            _db = db;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return null;
            // Manual mapping example
            return appUser;
        }

        public async Task<ApplicationUser?> UpdatePersonalInfoAsync(Guid userId, UpdatePersonalInfoRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");
            user.FullName = request.FullName;
            user.Region = request.Region;
            user.DateOfBirth = request.Birthdate;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));
            await _userPublisher.PublishUserUpdated(user.ToUserAdminDTO());
            return user;
        }
        public async Task<ICollection<UserAddress>?> GetUserAddressesAsync(Guid userId)
        {
            return await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task AddAddressAsync(Guid userId, UserAddress address)
        {
            address.UserId = userId;
            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(Guid addressId, UserAddress address)
        {
            var existingAddress = await _db.Addresses.FindAsync(addressId);
            if (existingAddress == null) throw new InvalidOperationException("Address not found.");
            existingAddress.Street = address.Street;
            existingAddress.City = address.City;
            existingAddress.State = address.State;
            existingAddress.Country = address.Country;
            existingAddress.IsDefault = address.IsDefault;
            _db.Addresses.Update(existingAddress);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(Guid userId, Guid addressId)
        {
            var address = await _db.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);
            var addresses = await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
            if (address == null) throw new InvalidOperationException("Address not found.");
            if(address.IsDefault == true)
            {
                if(addresses.Count > 1)
                {
                    var newDefaultAddress = addresses.FirstOrDefault(a => a.AddressId != addressId);
                    if (newDefaultAddress != null)
                    {
                        newDefaultAddress.IsDefault = true;
                    }
                }
            }
            _db.Addresses.Remove(address);
            await _db.SaveChangesAsync();
        }
        public async Task SetAddressDefaultAsync(Guid userId, Guid addressId)
        {
            var addresses = await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
            foreach (var addr in addresses)
            {
                addr.IsDefault = addr.AddressId == addressId;
            }
            await _db.SaveChangesAsync();
        }



        public async Task<ICollection<UserPhone>?> GetUserPhonesAsync(Guid userId)
        {
            return await _db.Phones.Where(p => p.UserId == userId).ToListAsync();
        }


        public async Task AddPhoneAsync(Guid userId, UserPhone phone)
        {
            phone.UserId = userId;
            _db.Phones.Add(phone);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePhoneAsync(Guid userId, UserPhone phone)
        {
            var existingPhone = await _db.Phones.FirstOrDefaultAsync(p => p.UserId == userId && p.PhoneId == phone.PhoneId);
            if (existingPhone == null) throw new InvalidOperationException("Phone not found.");
            existingPhone.PhoneNumber = phone.PhoneNumber;
            existingPhone.IsDefault = phone.IsDefault;
            _db.Phones.Update(existingPhone);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePhoneAsync(Guid userId, Guid phoneId)
        {
            var phone = await _db.Phones.FirstOrDefaultAsync(p => p.UserId == userId && p.PhoneId == phoneId);
            var phones = await _db.Phones.Where(p => p.UserId == userId).ToListAsync();
            if (phone == null) throw new InvalidOperationException("Phone not found.");
            if(phone.IsDefault == true)
            {
                if(phones.Count > 1)
                {
                    var newDefaultPhone = phones.FirstOrDefault(p => p.PhoneId != phoneId);
                    if (newDefaultPhone != null)
                    {
                        newDefaultPhone.IsDefault = true;
                    }
                }
            }
            _db.Phones.Remove(phone);
            await _db.SaveChangesAsync();
        }

        public async Task SetPhoneDefaultAsync(Guid userId, Guid phoneId)
        {
            var phones = await _db.Phones.Where(a => a.UserId == userId).ToListAsync();
            foreach (var phone in phones)
            {
                phone.IsDefault = phone.PhoneId == phoneId;
            }
            await _db.SaveChangesAsync();
        }
    }
}
