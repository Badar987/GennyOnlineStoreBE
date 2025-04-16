using GennyOnlineStoreBE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GennyOnlineStoreBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //baseurl/api/UserAuth
    public class UserAuthController : ControllerBase
    {
        private UserManager<ApplicationUsers> _userManager;
        private SignInManager<ApplicationUsers> _signinManager;
        private string? _jwtKey;
        private string? _issuer;
        private string? _audience;
        private int _ExpiryMinutes;

        public UserAuthController(UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager, IConfiguration configuration, SignInManager<ApplicationUsers> signinManager)
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _jwtKey = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _ExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"]);
        }

        //baseUrl/api/UserAuth/Register (In the last method Name)
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel == null || string.IsNullOrEmpty(registerModel.Name) || string.IsNullOrEmpty(registerModel.Email) || string.IsNullOrEmpty(registerModel.Password))
            {
                return BadRequest("Invalid Registration Detail");
            }

            var existingUser = await _userManager.FindByEmailAsync(registerModel.Email);

            if (existingUser != null)
            {
                return Conflict("User Alrady Exist");
            }

            var user = new ApplicationUsers
            {
                UserName = registerModel.Name,
                Email = registerModel.Email,
                Name = registerModel.Name,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("User Created");

        }


        //baseUrl/Api/UserAuth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null) { 
            return BadRequest( new {success = false , message = "Invalid UserName and Password"});
            }

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Invalid UserName and Password" });
            }

            var token = GenerateJwtToken(user);

            return Ok(token);
        }

        private string GenerateJwtToken(ApplicationUsers user)
        {
            return "AsliBabaChaliChor";
        }
    }
}
