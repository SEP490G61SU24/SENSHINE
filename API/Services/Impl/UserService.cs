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

        public async Task<User> AddUser(string? username = null, string? phone = null, string? password = null, string? firstName = null, string? midName = null, string? lastName = null, DateTime? birthDate = null, string? provinceCode = null, string? districtCode = null, string? wardCode = null, int? roleId = null)
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
                Phone = phone,
                Password = hashedPassword,
                FirstName = firstName,
                MidName = midName,
                LastName = lastName,
                BirthDate = birthDate,
                ProvinceCode = provinceCode,
                DistrictCode = districtCode,
                WardCode = wardCode
            };

            Role role;
            if (roleId.HasValue)
            {
                role = await _context.Roles.FindAsync(roleId.Value);
                if (role == null)
                {
                    throw new ArgumentException("Role not found.");
                }
            }
            else
            {
                role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "STAFF");
                if (role == null)
                {
                    throw new ArgumentException("Default role 'STAFF' not found.");
                }
            }


            user.Roles = new List<Role> { role };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(int id, UserDto userDto)
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

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await (from user in _context.Users.Include(u => u.Roles)
                               join ward in _context.Wards on user.WardCode equals ward.Code into wardsJoined
                               from ward in wardsJoined.DefaultIfEmpty()
                               join district in _context.Districts on ward.DistrictCode equals district.Code into districtsJoined
                               from district in districtsJoined.DefaultIfEmpty()
                               join province in _context.Provinces on district.ProvinceCode equals province.Code into provincesJoined
                               from province in provincesJoined.DefaultIfEmpty()
                               select new UserDto
                               {
                                   Id = user.Id,
                                   UserName = user.UserName,
                                   Password = user.Password,
                                   FirstName = user.FirstName,
                                   MidName = user.MidName,
                                   LastName = user.LastName,
                                   Phone = user.Phone,
                                   BirthDate = user.BirthDate,
                                   ProvinceCode = user.ProvinceCode,
                                   DistrictCode = user.DistrictCode,
                                   WardCode = user.WardCode,
                                   RoleId = user.Roles.Select(ur => ur.Id).FirstOrDefault(),
                                   RoleName = user.Roles.Select(ur => ur.RoleName).FirstOrDefault(),
                                   Address = $"{ward.Name ?? "-"} - {district.Name ?? "-"} - {province.Name ?? "-"}"
                               }).ToListAsync();


            return users;
        }

        public async Task<UserDto> GetById(int id)
        {
            var userDto = await (from user in _context.Users.Include(u => u.Roles)
                                 join ward in _context.Wards on user.WardCode equals ward.Code into wardsJoined
                                 from ward in wardsJoined.DefaultIfEmpty()
                                 join district in _context.Districts on ward.DistrictCode equals district.Code into districtsJoined
                                 from district in districtsJoined.DefaultIfEmpty()
                                 join province in _context.Provinces on district.ProvinceCode equals province.Code into provincesJoined
                                 from province in provincesJoined.DefaultIfEmpty()
                                 where user.Id == id
                                 select new UserDto
                                 {
                                     Id = user.Id,
                                     UserName = user.UserName,
                                     Password = user.Password,
                                     FirstName = user.FirstName,
                                     MidName = user.MidName,
                                     LastName = user.LastName,
                                     Phone = user.Phone,
                                     BirthDate = user.BirthDate,
                                     ProvinceCode = user.ProvinceCode,
                                     DistrictCode = user.DistrictCode,
                                     WardCode = user.WardCode,
                                     RoleId = user.Roles.Select(ur => ur.Id).FirstOrDefault(),
                                     RoleName = user.Roles.Select(ur => ur.RoleName).FirstOrDefault(),
                                     Address = $"{ward.Name ?? "-"} - {district.Name ?? "-"} - {province.Name ?? "-"}"
                                 }).FirstOrDefaultAsync();
            return userDto;
        }


        public async Task<User> GetByUserName(string username)
        {
            return await _context.Users
                                .Include(u => u.Roles)
                                .SingleOrDefaultAsync(u => u.UserName == username);
        }
    }
}
