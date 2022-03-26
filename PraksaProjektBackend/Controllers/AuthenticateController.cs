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
using PraksaProjektBackend.Services;
using PraksaProjektBackend.Models;

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
        private readonly IMailService _mailService;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,IMailService mailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return NotFound();
            }
            var userId = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
            if (userId != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "You are logged in" });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Hash, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);
                Response.Cookies.Append("token", new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("loggeduser")]
        [Authorize]
        public async Task<IActionResult> LoggedUser()
        {
            try
            {
                var jwt = Request.Cookies["token"];
                var token = Verify(jwt);
                var userId = token.Payload.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Hash)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                var userinfo = new UserInfo
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Role = await _userManager.GetRolesAsync(user),
                };

                return Ok(userinfo);

            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }




        [HttpGet]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            });
            return Ok(new Response { Status = "Success", Message = "Success" });

        }
        //[Authorize]
        //[HttpPost]
        //[Route("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //await _signInManager.SignOutAsync();
        //  return Unauthorized();
        //}
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


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return NotFound();
            }
            var userId = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
            if (userId != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "You are logged in" });
            }
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

        [Authorize]
        [HttpGet]
        [Route("editaccount")]
        public IActionResult EditAccount()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userMail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
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
            if (user == null || model.Id != userId)
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
        [Authorize]
        [HttpDelete]
        [Route("deleteaccount")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userClaims = identity.Claims;
                var role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value;
                var accountid = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Hash)?.Value;

                // users can delete their own account and admins can delete any account
                if (id != accountid && role != UserRoles.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = new List<string>();

                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = String.Join(",", errors) });
                }

                return Ok(new Response { Status = "Success", Message = "Account deleted successfully" });


            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found" });

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


        [Authorize]
        [HttpPost]
        [Route("changepassword")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null || model.Username != userName)
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

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword request)
        {
            if (string.IsNullOrEmpty(request.ToEmail))
                return NotFound();

            var result= await _mailService.SendEmailAsync(request);
                
            if (result.IsSuccess)
                return Ok(result); 

            return BadRequest(result);
        }

        [HttpPost]
        [Route("resetpassword")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "Password changed successfully!" });

                }
                else
                {
                    var errors = new List<string>();

                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = String.Join(",", errors) });
                }

            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User doesn't exist" });

        }

       // [HttpGet]
       // [Route("getcurrentuser")]
       //  public UserInfo GetCurrentUser()
       // {

       //     var identity = HttpContext.User.Identity as ClaimsIdentity;
       //     if (identity != null)
       //     {

       //         var userClaims = identity.Claims;


       //         return new UserInfo
       //         {

       //             Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
       //             Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
       //             Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value
       //         };
       //     }
       //     return null;
       // }
        private JwtSecurityToken Verify(string jwt)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;

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
