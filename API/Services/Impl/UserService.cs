﻿using API.Dtos;
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

        public async Task<User> AddUser(UserDTO userDto)
        {
            if (!string.IsNullOrEmpty(userDto.UserName))
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userDto.UserName);
                if (existingUser != null)
                {
                    throw new ArgumentException("Username already exists.");
                }
            }

            string hashedPassword = null;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                hashedPassword = PasswordUtils.HashPassword(userDto.Password);
            }

            var user = new User
            {
                UserName = userDto.UserName,
                Phone = userDto.Phone,
                Password = hashedPassword,
                FirstName = userDto.FirstName,
                MidName = userDto.MidName,
                LastName = userDto.LastName,
                BirthDate = userDto.BirthDate,
                Status = "ACTIVE",
                StatusWorking = "INACTIVE",
                ProvinceCode = userDto.ProvinceCode,
                DistrictCode = userDto.DistrictCode,
                WardCode = userDto.WardCode
            };

            if(userDto.SpaId.HasValue)
            {
                user.SpaId = userDto.SpaId.Value;
            }

            Role role;
            if (userDto.RoleId.HasValue)
            {
                role = await _context.Roles.FindAsync(userDto.RoleId.Value);
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

        public async Task<User> UpdateUser(int id, UserDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = PasswordUtils.HashPassword(userDto.Password);
            }

            user.Phone = userDto.Phone;
            user.FirstName = userDto.FirstName;
            user.MidName = userDto.MidName;
            user.LastName = userDto.LastName;
            user.BirthDate = userDto.BirthDate;
            user.ProvinceCode = userDto.ProvinceCode;
            user.DistrictCode = userDto.DistrictCode;
            user.WardCode = userDto.WardCode;

            if (userDto.SpaId.HasValue)
            {
                user.SpaId = userDto.SpaId.Value;
            }

            user.Roles.Clear();

            Role role;
            if (userDto.RoleId != null)
            {
                role = await _context.Roles.FindAsync(userDto.RoleId);
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

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await (from user in _context.Users.Include(u => u.Roles).Where(u => u.Roles.All(r => r.Id != 5))
							   join ward in _context.Wards on user.WardCode equals ward.Code into wardsJoined
                               from ward in wardsJoined.DefaultIfEmpty()
                               join district in _context.Districts on ward.DistrictCode equals district.Code into districtsJoined
                               from district in districtsJoined.DefaultIfEmpty()
                               join province in _context.Provinces on district.ProvinceCode equals province.Code into provincesJoined
                               from province in provincesJoined.DefaultIfEmpty()
                               select new UserDTO
                               {
                                   Id = user.Id,
                                   UserName = user.UserName,
                                   Password = user.Password,
                                   FirstName = user.FirstName,
                                   MidName = user.MidName,
                                   LastName = user.LastName,
                                   Phone = user.Phone,
                                   BirthDate = user.BirthDate,
                                   Status = user.Status,
                                   StatusWorking = user.StatusWorking,
                                   SpaId = user.SpaId,
                                   ProvinceCode = user.ProvinceCode,
                                   DistrictCode = user.DistrictCode,
                                   WardCode = user.WardCode,
                                   RoleId = user.Roles.Select(ur => ur.Id).FirstOrDefault(),
                                   RoleName = user.Roles.Select(ur => ur.RoleName).FirstOrDefault(),
                                   Address = $"{ward.Name ?? "-"} - {district.Name ?? "-"} - {province.Name ?? "-"}"
                               }).ToListAsync();


            return users;
        }

        public async Task<UserDTO> GetById(int id)
        {
            var userDto = await (from user in _context.Users.Include(u => u.Roles)
                                 join ward in _context.Wards on user.WardCode equals ward.Code into wardsJoined
                                 from ward in wardsJoined.DefaultIfEmpty()
                                 join district in _context.Districts on ward.DistrictCode equals district.Code into districtsJoined
                                 from district in districtsJoined.DefaultIfEmpty()
                                 join province in _context.Provinces on district.ProvinceCode equals province.Code into provincesJoined
                                 from province in provincesJoined.DefaultIfEmpty()
                                 where user.Id == id
                                 select new UserDTO
                                 {
                                     Id = user.Id,
                                     UserName = user.UserName,
                                     Password = user.Password,
                                     FirstName = user.FirstName,
                                     MidName = user.MidName,
                                     LastName = user.LastName,
                                     Phone = user.Phone,
                                     BirthDate = user.BirthDate,
                                     Status = user.Status,
                                     StatusWorking = user.StatusWorking,
                                     SpaId = user.SpaId,
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

        public async Task<string> GetAddress(string wardCode, string districtCode, string provinceCode)
        {
            var ward = await _context.Wards.SingleOrDefaultAsync(w => w.Code == wardCode);
            var district = await _context.Districts.SingleOrDefaultAsync(d => d.Code == districtCode);
            var province = await _context.Provinces.SingleOrDefaultAsync(p => p.Code == provinceCode);

            var wardName = ward?.Name ?? "-";
            var districtName = district?.Name ?? "-";
            var provinceName = province?.Name ?? "-";

            var addressString = $"{wardName} - {districtName} - {provinceName}";
            return addressString;
        }
    }
}
