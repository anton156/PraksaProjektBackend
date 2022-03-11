using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PraksaProjektBackend.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
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
                    new Claim(ClaimTypes.Hash, user.Id),
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
        //google login
        //[Route("google-login")]
        //public IActionResult GoogleLogin()
        //{
        //    string redirectUrl = Url.Action("GoogleResponse", "Account");
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        //    return new ChallengeResult("Google", properties);
        //}

        //[Route("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //        return RedirectToAction(nameof(Login));

        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        //    string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
        //    if (result.Succeeded)
        //        return Ok(new Response { Status = "Success", Message = String.Join(",", userInfo) });
        //    else
        //    {
        //        ApplicationUser user = new ApplicationUser
        //        {
        //            Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
        //            UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
        //        };

        //        IdentityResult identResult = await _userManager.CreateAsync(user);
        //        if (identResult.Succeeded)
        //        {
        //            identResult = await _userManager.AddLoginAsync(user, info);
        //            if (identResult.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, false);
        //                return Ok(new Response { Status = "Success", Message = String.Join(",", userInfo) });
        //            }
        //        }
        //        return Unauthorized();
        //    }
        //}
        //[HttpPost]
        //[Route("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //     await signInManager.SignOutAsync();
        //    return Unauthorized();
        //}
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
        
        
        //legacy edituser get
        //[HttpGet]
        //[Route("editaccount")]
        //public async Task<ActionResult<ApplicationUser>> EditAccount(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);

        //    if (user == null)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not allowed" });
        //    }

        //    var model = new EditAccountModel
        //    {
        //        Id = user.Id,
        //        Email = user.Email,
        //        Firstname = user.FirstName,
        //        Lastname = user.LastName,
        //        Address = user.Address,
        //        PhoneNumber = user.PhoneNumber,
        //        Username = user.UserName
        //    };

        //    return user;
        //}


        [HttpGet]
        [Route("editaccount")]
        public IActionResult EditAccount()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userMail = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            if (userMail == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Not found" });
            }
            else
            {
                ApplicationUser user = _userManager.FindByEmailAsync(userMail).Result;
                return Ok(user);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("editaccount")]
        public async Task<IActionResult> EditAccount(EditAccountModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
            if (user == null && model.Id != userId)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not allowed" });
            }
            else
            {
                user.Email = model.Email;
                user.FirstName = model.Firstname;
                user.LastName = model.Lastname;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.Username;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "User Updated successfully!" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return Ok(new Response { Status = "Success", Message = "User Up successfully!" });
            }
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
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not found" });
        }

        [HttpPost]
        [Route("changepassword")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User doesn't exist" });
            }

            if(string.Compare(model.NewPassword, model.ConfirmNewPassword)!=0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "New password and confirm password does not match" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach(var error in result.Errors)
                {
                    errors.Add(error.Description);

                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = String.Join(",",errors) });

            }

            return Ok(new Response { Status = "Success", Message = "Password changed successfully" });
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
