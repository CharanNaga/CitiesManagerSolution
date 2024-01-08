using CitiesManager.Core.DTO;
using CitiesManager.Core.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CitiesManager.WebAPI.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                string error = string.Join(
                    "|",ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return Problem(error);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.Phone
            };

            IdentityResult result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(user);
            }
            else
            {
                string errorMessage = string.Join(
                    "|", result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }
        }
        [HttpGet]
        [AllowAnonymous] //f commented this, it throws 401 error.
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result != null)
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }
        }
    }
}
