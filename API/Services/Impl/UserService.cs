using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            var user = await _context.Users
                                     .Include(r => r.Roles)
                                     .SingleOrDefaultAsync(u => u.UserName == username || u.Phone == username);

            if (user == null || !PasswordUtils.VerifyPassword(password, user.Password))
            {
                return null;
            }

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> AddUser(UserDTO userDto)
        {
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
                    throw new InvalidOperationException("Không tìm thấy vai trò.");
                }
            }
            else
            {
                role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "STAFF");
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
                user.Password = PasswordUtils.HashPassword(userDto.Password);
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
                role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "STAFF");
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

        public async Task<IEnumerable<UserDTO>> GetUsersByRole(int roleId)
        {
            var users = await _context.Users
                                 .Include(u => u.Roles)
                                 .Where(u => u.Roles.Any(r => r.Id == roleId))
                                 .ToListAsync();
            if (users == null)
            {
                return Enumerable.Empty<UserDTO>();
            }
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<PaginatedList<UserDTO>> GetUsersByRoleWithPage(int roleId, int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            // Tạo query cơ bản
            IQueryable<User> query = _context.Users.Include(u => u.Roles).Where(u => u.Roles.Any(r => r.Id == roleId));

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
            var users = await (from user in _context.Users.Include(u => u.Roles).Where(u => u.Roles.All(r => r.Id != 5))
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

        public async Task<PaginatedList<UserDTO>> GetUsers(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            // Tạo query cơ bản
            IQueryable<User> query = _context.Users.Include(u => u.Roles).Where(u => u.Roles.All(r => r.Id != 5));

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

            if(userChange)
            {
			    if (!PasswordUtils.VerifyPassword(currentPassword, user.Password))
			    {
				    throw new InvalidOperationException("Mật khẩu hiện tại không chính xác.");
			    }
            }

			user.Password = PasswordUtils.HashPassword(newPassword);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
