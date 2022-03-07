using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PraksaProjektBackend.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already exists!" });

            var user = new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            
            await _userManager.AddToRoleAsync(user, UserRoles.Customer);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            await _userManager.AddToRoleAsync(user, UserRoles.Admin);


            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("register-organizer")]
        public async Task<IActionResult> RegisterOrganizer([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            await _userManager.AddToRoleAsync(user, UserRoles.Organizer);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("addtoorganizer")]
        public async Task<IActionResult> AddToOrganizer([FromBody] AddToOrganizer model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, UserRoles.Customer)){
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Customer);
                    await _userManager.AddToRoleAsync(user, UserRoles.Organizer);                   
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Organizer))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Already in the role" });
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
                    await _userManager.AddToRoleAsync(user, UserRoles.Organizer);
                }
                return Ok(new Response { Status = "Success", Message = "User added as organizer!" });
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found" });
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("removefromorganizeroradmin")]
        public async Task<IActionResult> RemoveFromOrganizerOrAdmin([FromBody] AddToOrganizer model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, UserRoles.Customer))
                {
                    
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not in role" });
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Organizer))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Organizer);
                    await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
                    await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                }
                return Ok(new Response { Status = "Success", Message = "User removed from role!" });
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
