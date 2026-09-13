using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using TodoAPI.Models;
using TodoAPI.Data;
using TodoAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/user/register
        [HttpPost("register")]
        public ActionResult<ApiResponse<User>> Register([FromBody] UserRegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Invalid input data",
                    Success = false,
                    Data = null
                });
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Message = "Username already exists",
                    Success = false,
                    Data = null
                });
            }


            var _UserId = Guid.NewGuid();

            var user = new User
            {
                UserId = _UserId,
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userRole = new UserRole
            {
                RoleId = model.RoleId,
                UserId = _UserId
            };

            _context.UserRoles.Add(userRole);
            _context.Users.Add(user);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<User>
                {
                    Message = "Error registering user",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "User registered successfully",
                Success = true,
                Data = user.UserId
            });
        }

        // POST: api/user/login
        [HttpPost("login")]
        public ActionResult<ApiResponse<object>> Login([FromBody] UserLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Invalid input data",
                    Success = false,
                    Data = null
                });
            }

            var user = _context.Users.Include(u => u.RefreshTokens)
                .SingleOrDefault(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Message = "Invalid credentials",
                    Success = false,
                    Data = null
                });
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = user.UserId,
            });

            _context.UserAudits.Add(new UserAudit { LoginTime = DateTime.UtcNow, UserId = user.UserId });

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = "Error logging in",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Login successful",
                Success = true,
                Data = new
                {
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,
                    UserId = user.UserId,
                }
            });
        }

        [HttpGet("roles/{userId}")]
        public ActionResult<ApiResponse<List<string>>> GetUserRoles(Guid userId)
        {
            try
            {
                // Select role names for the user, joining UserRole and Role entities
                var roles = _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.Role.RoleName) // Select RoleName instead of RoleId
                    .ToList();

                if (!roles.Any())
                {
                    return NotFound(new ApiResponse<List<string>>
                    {
                        Success = false,
                        Message = "User has no roles assigned",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Message = "Roles retrieved successfully",
                    Data = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving roles",
                    Data = null
                });
            }
        }


        // POST: api/user/logout/{userId}
        [HttpPost("logout/{UserId}")]
        public IActionResult Logout(Guid UserId)
        {
            try
            {
                // Find the last audit entry for the user (the most recent login without a logout time)
                var lastAudit = _context.UserAudits
                    .Where(ua => ua.UserId == UserId && ua.LogoutTime == null)
                    .OrderByDescending(ua => ua.LoginTime)
                    .FirstOrDefault();

                if (lastAudit == null)
                {
                    return BadRequest(new ApiResponse<UserAudit>
                    {
                        Message = "No active login session found for this user",
                        Success = false,
                        Data = null
                    });
                }

                // Set the logout time to the current time
                lastAudit.LogoutTime = DateTime.UtcNow;
                _context.UserAudits.Update(lastAudit);
                _context.SaveChanges();

                return Ok(new ApiResponse<UserAudit>
                {
                    Message = "Logout recorded successfully",
                    Success = true,
                    Data = lastAudit
                });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new ApiResponse<UserAudit>
                {
                    Message = "An error occurred while processing your request.",
                    Success = false,
                    Data = null
                });
            }
        }

        // POST: api/user/refresh-token
        [HttpPost("refresh-token")]
        public ActionResult<ApiResponse<string>> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Refresh token is required"
                });
            }

            var user = _context.Users.Include(u => u.RefreshTokens)
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                });
            }

            var storedToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

            if (!storedToken.IsActive)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Token expired or revoked"
                });
            }

            // Generate new JWT and refresh token
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Revoke the old refresh token
            storedToken.Revoked = DateTime.UtcNow;

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = user.UserId
            });

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error refreshing token"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = newJwtToken
            });
        }

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate refresh token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        // GET: api/role
        [HttpGet("Role/")]
        public ActionResult<ApiResponse<List<Role>>> GetAllRoles()
        {
            try
            {
                var roles = _context.Roles.ToList();

                if (roles == null || !roles.Any())
                {
                    return NotFound(new ApiResponse<List<Role>>
                    {
                        Success = false,
                        Message = "No roles found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<Role>>
                {
                    Success = true,
                    Message = "Roles retrieved successfully",
                    Data = roles
                });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logger here)
                return StatusCode(500, new ApiResponse<List<Role>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving roles",
                    Data = null
                });
            }
        }
    }
}
