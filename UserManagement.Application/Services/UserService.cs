using System;
using System.Net;
using System.Threading.Tasks;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Mappers.UserMappers;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Application.ServiceContracts;

namespace UserManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            var user = await _userRepo.GetUserByEmailAsync(email);
            return user?.ToUserDTO();
        }

        public async Task<UserDTO?> UpdatePersonalInfoAsync(Guid? userId, UpdatePersonalInfoRequest request)
           {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("FullName cannot be null or empty.", nameof(request.FullName));
            if (request.Birthdate == DateTime.MinValue)
                throw new ArgumentException("Birthdate cannot be null or empty.", nameof(request.Birthdate));
            request.Region ??= "Egypt"; // Default value if region is null
            var user = await _userRepo.UpdatePersonalInfoAsync(userId.Value, request); 
            return user?.ToUserDTO();

        }


        //address methods
        public async Task<ICollection<AddressDTO>?> GetUserAddressesAsync(Guid? userId)
        { 
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
                var addresses = await _userRepo.GetUserAddressesAsync(userId.Value);
                return addresses?.Select(ua => ua.ToUserAddressDTO()).ToList();
        }


        public async Task AddAddressAsync(Guid? userId, AddressDTO? address)
        {   if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            if (address == null)
                throw new ArgumentNullException(nameof(address), "Address cannot be null.");
            var addressEntity = address.ToUserAddress();
            addressEntity.AddressId = Guid.NewGuid();
            await _userRepo.AddAddressAsync(userId.Value, addressEntity);
            if(address.IsDefault == true)
            {
                await _userRepo.SetAddressDefaultAsync(userId.Value, addressEntity.AddressId);
            }
        }

        public async Task UpdateAddressAsync(Guid? userId, AddressDTO? address)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("userId cannot be null or empty.", nameof(userId));

            if (address == null)
                throw new ArgumentNullException(nameof(address), "Address cannot be null.");
            var addressEntity = address.ToUserAddress();
            await _userRepo.UpdateAddressAsync(userId.Value, addressEntity);
            if (address.IsDefault == true)
            {
                await _userRepo.SetAddressDefaultAsync(userId.Value, addressEntity.AddressId);
            }
        }

        public async Task DeleteAddressAsync(Guid? userId, Guid? addressId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (addressId == null || addressId == Guid.Empty)
                throw new ArgumentException("AddressId cannot be null or empty.", nameof(addressId));

            await _userRepo.DeleteAddressAsync(userId.Value, addressId.Value);
        }

        public async Task SetAddressDefaultAsync(Guid? userId, Guid? addressId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (addressId == null || addressId == Guid.Empty)
                throw new ArgumentException("AddressId cannot be null or empty.", nameof(addressId));

            await _userRepo.SetAddressDefaultAsync(userId.Value, addressId.Value);
        }


        //phone methods
        public async Task<ICollection<PhoneDTO>?> GetUserPhonesAsync(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            var phones = await _userRepo.GetUserPhonesAsync(userId.Value);
            return phones?.Select(up => up.ToUserPhoneDTO()).ToList();
        }

        public async Task AddPhoneAsync(Guid? userId, PhoneDTO? phone)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (phone == null)
                throw new ArgumentNullException(nameof(phone), "Phone cannot be null.");
            var phoneEntity = phone.ToUserPhone();
            phoneEntity.PhoneId = Guid.NewGuid();
            await _userRepo.AddPhoneAsync(userId.Value, phoneEntity);
            if (phone.IsDefault == true)
            {
                await _userRepo.SetPhoneDefaultAsync(userId.Value, phoneEntity.PhoneId);
            }
        }
        public async Task UpdatePhoneAsync(Guid? userId, PhoneDTO? phone)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (phone == null)
                throw new ArgumentNullException(nameof(phone), "Phone cannot be null.");

            var phoneEntity = phone.ToUserPhone();
            await _userRepo.UpdatePhoneAsync(userId.Value, phoneEntity);
            if (phone.IsDefault == true)
            {
                await _userRepo.SetAddressDefaultAsync(userId.Value, phoneEntity.PhoneId);
            }
        }

        public async Task DeletePhoneAsync(Guid? userId, Guid? phoneId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (phoneId == null || phoneId == Guid.Empty)
                throw new ArgumentException("PhoneId cannot be null or empty.", nameof(phoneId));

            await _userRepo.DeletePhoneAsync(userId.Value, phoneId.Value);
        }
        public async Task SetPhoneDefaultAsync(Guid? userId, Guid? phoneId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            if (phoneId == null || phoneId == Guid.Empty)
                throw new ArgumentException("PhoneId cannot be null or empty.", nameof(phoneId));
            await _userRepo.SetPhoneDefaultAsync(userId.Value, phoneId.Value);
        }
       
    }
}
