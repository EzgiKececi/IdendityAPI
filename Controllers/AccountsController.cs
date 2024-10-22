using IdentityAPI.Dtos;
using IdentityAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace IdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserEntitiy user)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState.Values.SelectMany(e=>e.Errors).Select(x=>x.ErrorMessage));

            var newUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return Ok("Kayıt başarılı");
            }

            return BadRequest(new {errors = result.Errors.Select(e=>e.Description)});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors).Select(x => x.ErrorMessage));

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return Ok("Giriş başarılı");
            }
            else
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı");
            }
        }


        [HttpPost("createrole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if(!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return Ok("Rol oluşturuldu.");
                }
                else
                    return BadRequest(new {errors = result.Errors.Select(e=> e.Description)});               
            }
            return BadRequest(new { message = "Rol adı boş olamaz!" });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = _roleManager.Roles.ToList();

            return Ok(result);
        }    


    }
}
