using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO model)
        {
            var user = await _userService.Authenticate(model.UserName, model.Password);

            if (user == null)
                return Unauthorized();

            var tokenString = GenerateJwtToken(user);
            return Ok(
                new { 
                        token = tokenString,
                        username = user.UserName,
                        id = user.Id
                    }
                );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO model)
        {
            var user = await _userService.AddUser(model);

            if (user == null)
                return Unauthorized();

            var tokenString = GenerateJwtToken(user);
            return Ok(new { Token = tokenString });
        }

		[HttpPost("changepass")]
		public async Task<IActionResult> ChangePass(ChangePasswordDTO model)
		{
			try
			{
				if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
				{
					return BadRequest("Thiếu dữ liệu đầu vào!");
				}

				var result = await _userService.ChangePassword(model.UserName, model.OldPassword, model.NewPassword, true);
				if (result)
				{
					return Ok("Thay đổi mật khẩu thành công!");
				}
				else
				{
					return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra !");
				}
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
			}
		}

		private string GenerateJwtToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo danh sách các Claim bao gồm thông tin cơ bản và role của người dùng
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Lấy danh sách role của người dùng từ UserRoles
            var roles = user.Roles.Select(ur => ur.RoleName).ToList();

            // Thêm các role vào danh sách Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
               issuer: _configuration["Jwt:Issuer"],
               audience: _configuration["Jwt:Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(30),
               signingCredentials: credentials
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
