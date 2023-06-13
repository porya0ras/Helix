using helix.Data;
using helix.Models;
using helix.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace helix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<AuthenticateController> _Loger;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IConfiguration _Configuration;
        private readonly ApplicationDbContext _dbContext;
        public AuthenticateController(ILogger<AuthenticateController> loger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext)
        {
            _Loger=loger;
            _HttpContextAccessor=httpContextAccessor;
            _UserManager=userManager;
            _dbContext=dbContext;
            _Configuration=configuration;
            _RoleManager=roleManager;



        }
        [AllowAnonymous]
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _UserManager.FindByEmailAsync(register.EmailAddress);
            if (user != null)
            {
                return BadRequest($"user {register.EmailAddress} alredy exists!");
            }
            var newUser = new User()
            {
                FirstName=register.Firstname,
                LastName=register.Lastname,
                UserName=register.Username,
                Email=register.EmailAddress,
                SecurityStamp=Guid.NewGuid().ToString()
            };
            var result = await _UserManager.CreateAsync(newUser, register.Password);
            if (result.Succeeded)
            {
                var _user = await _UserManager.FindByEmailAsync(register.EmailAddress);

                if (_user!=null)
                {
                    var roleresult = await _UserManager.AddToRoleAsync(_user, UserType.GENERAL.ToString());

                    if (roleresult.Succeeded)
                    {
                        return Ok("User Created.");
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                else
                {
                    return BadRequest("SomeThing is Wrong!");
                }
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }
        [AllowAnonymous]
        [HttpPost("login-user")]
        public async Task<IActionResult> Login([Bind(include: "UserName,Password")] LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _UserManager.FindByNameAsync(login.UserName);
            if (user != null && await _UserManager.CheckPasswordAsync(user, login.Password))
            {
                var roles = await _UserManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    return Ok(await GenerateJwtTokenAsync(user, roles.First()));
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        private async Task<AuthVModel> GenerateJwtTokenAsync(User appUser, string role)
        {
            try
            {
                var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appUser.FirstName+" "+appUser.LastName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Role,role),
                new Claim(JwtRegisteredClaimNames.Email,appUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub,appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

            };

                var IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _Configuration["JWT:Issuer"],
                    audience: _Configuration["JWT:Audience"],
                    claims: authClaim,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return new AuthVModel()
                {
                    Token=jwtToken,
                    ExpireAt=token.ValidTo
                };
            }
            catch(Exception ex)
            {
                return new AuthVModel();
            }
        }
    }
}
