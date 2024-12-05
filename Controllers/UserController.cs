using DeliveryAPI.Data;
using DeliveryAPI.Models.Domain;
using DeliveryAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DeliveryDbContext dbContext;
        private IConfiguration _config;
        public UserController(DeliveryDbContext dbContext, IConfiguration config)
        {
            this.dbContext = dbContext;
            _config = config;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(AddUserViewModel addUserRequest)
        {
            try
            {
                var user = new User()
                {
                    UserId = Guid.NewGuid(),
                    FullName = addUserRequest.FullName,
                    BirthDate = addUserRequest.BirthDate,
                    Gender = addUserRequest.Gender,
                    Address = addUserRequest.Address,
                    Email = addUserRequest.Email,
                    PhoneNumber = addUserRequest.PhoneNumber,
                    Password = addUserRequest.Password,
                };
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();

                var token = GenerateJwtToken(user.UserId.ToString(), _config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"]);
                return Ok(new TokenResponse (){ Token = token });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel request)
        {
            try
            {
                var user = await dbContext.Users.Where(x => x.Email == request.Email && x.Password == request.Password).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound();
                }
                var token = GenerateJwtToken(user.UserId.ToString(), _config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"]);
                return Ok(new TokenResponse() { Token = token });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            List<Claim> claims = User.Claims.ToList();
            var check = claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            claims.Remove(check);
            var userIdentity = new ClaimsIdentity(claims, ClaimTypes.Name);
            //(User.Identity as ClaimsIdentity).RemoveClaim();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaim != null)
                {
                    var user = await dbContext.Users.Where(x => x.UserId.ToString() == userClaim.Value).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return Ok(user);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }

        [HttpPut]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> UpdateProfile(EditUserViewModel editUserRequest)
        {
            try
            {
                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaim != null)
                {
                    var user = await dbContext.Users.Where(x => x.UserId.ToString() == userClaim.Value).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.FullName = editUserRequest.FullName;
                    user.BirthDate = editUserRequest.BirthDate;
                    user.Gender = editUserRequest.Gender;
                    user.Address = editUserRequest.Address;
                    user.PhoneNumber = editUserRequest.PhoneNumber;
                    await dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }


        //JWT
        public static string GenerateJwtToken(string userId, string secretKey, string issuer, string audience)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            //new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier,userId)
        };
            
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token expiry
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
