using GennyOnlineStoreBE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GennyOnlineStoreBE.Controllers
{
    //https://.postman.co/workspace/My-Workspace~85ca7cfa-9a1c-4e11-8d5c-4eaa58bc3b9b/collection/33668791-bc1904cd-9f61-4170-b594-18072ceeee4d?action=share&creator=33668791
    //Envirnment Link To test api on Postman
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

            return Ok(new {sucess = true, token});
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return Ok("User LogOut Out Successfully");
        }
        private string GenerateJwtToken(ApplicationUsers user)
        {
            var Claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Name",user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.Now.AddMinutes(_ExpiryMinutes),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
