﻿using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public UserService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDTO> Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Tên người dùng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Mật khẩu không được để trống.");
            }

            var user = await _context.Users
                                     .Include(r => r.Roles)
                                     .SingleOrDefaultAsync(u => u.UserName == username || u.Phone == username);

            if (user == null || !PasswordUtils.VerifyPassword(password, user.Password))
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng.");
            }

            var userDto = _mapper.Map<UserDTO>(user);

            if (userDto.RoleId == (int)UserRoleEnum.CUSTOMER)
            {
                throw new InvalidOperationException("Khách hàng không thể đăng nhập hệ thống!");
            }

            if (userDto.Status != UserStatusEnum.ACTIVE.ToString())
            {
                throw new InvalidOperationException("Người dùng hiện tại không thể truy cập!");
            }

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> AddUser(UserDTO userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto), "Dữ liệu người dùng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                throw new ArgumentException("Tên người dùng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new ArgumentException("Mật khẩu không được để trống.");
            }

			if (!IsValidAge(userDto.BirthDate, userDto.RoleId ?? (int)UserRoleEnum.CUSTOMER))
			{
				throw new ArgumentException("Người dùng phải đủ 18 tuổi.");
			}

			if (!string.IsNullOrEmpty(userDto.UserName))
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userDto.UserName);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Tên người dùng đã tồn tại.");
                }
            }

            if (!string.IsNullOrEmpty(userDto.Phone))
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Phone == userDto.Phone);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Số điện thoại đã tồn tại.");
                }
            }

            string hashedPassword = null;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
				if (!IsValidPassword(userDto.Password))
				{
					throw new ArgumentException("Mật khẩu yếu, vui lòng thử lại.");
				}
				else
				{
				    hashedPassword = PasswordUtils.HashPassword(userDto.Password);
				}
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
                Status = UserStatusEnum.ACTIVE.ToString(),
                StatusWorking = UserWorkingStatusEnum.AVAILABLE.ToString(),
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
                    throw new InvalidOperationException("Không tìm thấy vai trò.");
                }
            }
            else
            {
                role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == UserRoleEnum.STAFF.ToString());
                if (role == null)
                {
                    throw new InvalidOperationException("Vai trò mặc định 'STAFF' không tìm thấy.");
                }
            }

            user.Roles = new List<Role> { role };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUser(int id, UserDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
				if (!IsValidPassword(userDto.Password))
				{
					throw new ArgumentException("Mật khẩu yếu, vui lòng thử lại.");
				} else
                {
				    user.Password = PasswordUtils.HashPassword(userDto.Password);
                }
            }

			if (!IsValidAge(userDto.BirthDate, userDto.RoleId ?? (int)UserRoleEnum.CUSTOMER))
			{
				throw new ArgumentException("Người dùng phải đủ 18 tuổi.");
			}

			if (!string.IsNullOrEmpty(userDto.Phone))
            {
                bool phoneExists = await _context.Users
                    .Where(u => u.Phone == userDto.Phone && u.Id != id)
                    .AnyAsync();

                if (phoneExists)
                {
                    throw new InvalidOperationException("Số điện thoại đã tồn tại.");
                }

                user.Phone = userDto.Phone;
            }

            user.FirstName = userDto.FirstName;
            user.MidName = userDto.MidName;
            user.LastName = userDto.LastName;
            user.BirthDate = userDto.BirthDate;
            user.ProvinceCode = userDto.ProvinceCode;
            user.DistrictCode = userDto.DistrictCode;
            user.WardCode = userDto.WardCode;

			if (!string.IsNullOrEmpty(userDto.Status))
			{
				user.Status = userDto.Status;
			}

			if (!string.IsNullOrEmpty(userDto.StatusWorking))
			{
				user.StatusWorking = userDto.StatusWorking;
			}

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
                    throw new InvalidOperationException("Không tìm thấy vai trò.");
                }
            }
            else
            {
                role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == UserRoleEnum.STAFF.ToString());
                if (role == null)
                {
                    throw new InvalidOperationException("Vai trò mặc định 'STAFF' không tìm thấy.");
                }
            }

            user.Roles = new List<Role> { role };

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDTO>(user);
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

        public async Task<IEnumerable<UserDTO>> GetUsersByRole(int roleId, string spaId = null)
        {
            int? spaIdInt = spaId != null && spaId != "ALL"
             ? int.Parse(spaId)
             : (int?)null;

            var query = _context.Users.Include(u => u.Roles)
                              .Where(u => u.Roles.Any(r => r.Id == roleId));

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.SpaId == spaIdInt.Value);
            }

            var users = await query.ToListAsync();

            if (users == null)
            {
                return Enumerable.Empty<UserDTO>();
            }

            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<PaginatedList<UserDTO>> GetUsersByRoleWithPage(int roleId, int pageIndex = 1, int pageSize = 10, string searchTerm = null, string spaId = null)
        {
            // Tạo query cơ bản
            IQueryable<User> query = _context.Users.Include(u => u.Roles).Where(u => u.Roles.Any(r => r.Id == roleId));

            int? spaIdInt = spaId != null && spaId != "ALL"
             ? int.Parse(spaId)
             : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.SpaId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.Contains(searchTerm) ||
                                         u.Phone.Contains(searchTerm) ||
                                         u.FirstName.Contains(searchTerm) ||
                                         u.LastName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var users = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);

            return new PaginatedList<UserDTO>
            {
                Items = userDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await (from user in _context.Users.Include(u => u.Roles).Where(u => u.Roles.All(r => r.Id != (int) UserRoleEnum.CUSTOMER))
                               join ward in _context.Wards on user.WardCode equals ward.Code into wardsJoined
                               from ward in wardsJoined.DefaultIfEmpty()
                               join district in _context.Districts on ward.DistrictCode equals district.Code into districtsJoined
                               from district in districtsJoined.DefaultIfEmpty()
                               join province in _context.Provinces on district.ProvinceCode equals province.Code into provincesJoined
                               from province in provincesJoined.DefaultIfEmpty()
                               select user).ToListAsync();

            if (users == null)
            {
                return Enumerable.Empty<UserDTO>();
            }

            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<PaginatedList<UserDTO>> GetUsers(int pageIndex = 1, int pageSize = 10, string searchTerm = null, string spaId = null)
        {
            // Tạo query cơ bản
            IQueryable<User> query = _context.Users.Include(u => u.Roles).Where(u => u.Roles.All(r => r.Id != (int)UserRoleEnum.CUSTOMER));

            int? spaIdInt = spaId != null && spaId != "ALL"
             ? int.Parse(spaId)
             : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.SpaId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.Contains(searchTerm) ||
                                         u.Phone.Contains(searchTerm) ||
                                         u.FirstName.Contains(searchTerm) ||
                                         u.LastName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var users = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);

            return new PaginatedList<UserDTO>
            {
                Items = userDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task<UserDTO> GetById(int id)
        {
            var user = await _context.Users
                          .Include(u => u.Roles)
                          .FirstOrDefaultAsync(u => u.Id == id);
            return _mapper.Map<UserDTO>(user);
        }
       
        public async Task<UserDTO> GetByUserName(string username)
        {
            var user = await _context.Users
                                .Include(u => u.Roles)
                                .SingleOrDefaultAsync(u => u.UserName == username);
            return _mapper.Map<UserDTO>(user);
        }
        public async Task<UserDTO2> GetByUserNameReturnRole(string username)
        {
            var user = await _context.Users
                             .Include(u => u.Roles)
                             .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return null; 
            }

            var userDTO = new UserDTO2
            {
               
                RoleName = user.Roles.Select(x => x.RoleName).SingleOrDefault()
            };


            return userDTO;
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

        public async Task<bool> ChangePassword(string userName, string currentPassword, string newPassword, bool userChange)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại.");
            }

            if (userChange)
            {
                if (!PasswordUtils.VerifyPassword(currentPassword, user.Password))
                {
                    throw new InvalidOperationException("Mật khẩu hiện tại không chính xác.");
                }
            }

            if (!IsValidPassword(newPassword))
            {
                throw new ArgumentException("Mật khẩu mới yếu, vui lòng thử lại.");
            }

            user.Password = PasswordUtils.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool IsValidPassword(string password)
        {
            var regex = new Regex("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{8,}");
            return regex.IsMatch(password);
        }

		private bool IsValidAge(DateTime? birthDate, int roleId)
		{
            if (roleId != (int)UserRoleEnum.CUSTOMER)
            {
				if (!birthDate.HasValue)
				{
					throw new ArgumentException("Ngày sinh không hợp lệ.");
				}

				int age = DateTime.Now.Year - birthDate.Value.Year;

				if (birthDate.Value.Date > DateTime.Now.AddYears(-age))
				{
					age--;
				}

				return age >= 18;
			}

            return true;
		}

        public async Task<IEnumerable<UserDTO>> GetCustomerByName(string name)
        {
            var usersQuery = _context.Users
                                     .Include(u => u.Roles)
                                     .Where(u => u.Roles.Any(r => r.Id == 5));

            if (!string.IsNullOrEmpty(name))
            {
                usersQuery = usersQuery.Where(u => (u.FirstName + " " + u.MidName + " " + u.LastName).Contains(name));
            }

            var users = await usersQuery.ToListAsync();

            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> UpdateUserWorkingStatus(int userId, string newStatusWorking)
        {
            if (!Enum.TryParse(typeof(UserWorkingStatusEnum), newStatusWorking, true, out var validStatus))
            {
                throw new ArgumentException("Trạng thái làm việc không hợp lệ.");
            }

            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại.");
            }

            user.StatusWorking = newStatusWorking;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

    }
}
