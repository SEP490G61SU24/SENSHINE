using API.Dtos;
using API.Models;
using API.Ultils;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly SenShineSpaContext _context;

        public UserService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _context.Users
                                     .Include(r => r.Roles)
                                     .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null || !PasswordUtils.VerifyPassword(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public async Task<User> AddUser(string? username = null, string? password = null, string? firstName = null, string? midName = null, string? lastName = null, DateTime? birthDate = null, string? provinceCode = null, string? districtCode = null, string? wardCode = null)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
                if (existingUser != null)
                {
                    throw new ArgumentException("Username already exists.");
                }
            }

            string hashedPassword = null;
            if (!string.IsNullOrEmpty(password))
            {
                hashedPassword = PasswordUtils.HashPassword(password);
            }

            var user = new User
            {
                UserName = username,
                Password = hashedPassword,
                FirstName = firstName,
                MidName = midName,
                LastName = lastName,
                BirthDate = birthDate,
                ProvinceCode = provinceCode,
                DistrictCode = districtCode,
                WardCode = wardCode
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            user.UserName = userDto.UserName;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = PasswordUtils.HashPassword(userDto.Password);
            }
            user.FirstName = userDto.FirstName;
            user.MidName = userDto.MidName;
            user.LastName = userDto.LastName;
            user.BirthDate = userDto.BirthDate;
            user.ProvinceCode = userDto.ProvinceCode;
            user.DistrictCode = userDto.DistrictCode;
            user.WardCode = userDto.WardCode;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersByRole(int roleId)
        {
            return await _context.Users
                                 .Include(u => u.Roles)
                                 .Where(u => u.Roles.Any(r => r.Id == roleId))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByUserName(string username)
        {
            return await _context.Users
                                .Include(u => u.Roles)
                                .SingleOrDefaultAsync(u => u.UserName == username);
        }
    }
}
