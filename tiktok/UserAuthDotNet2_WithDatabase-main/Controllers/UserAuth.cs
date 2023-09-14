
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.Extensions.Configuration;

// namespace User_Auth_.Net.Controllers
// {
//     [ApiController]
//     [Route("api/welcome")]
//     public class UserAuth : ControllerBase
//     {
//         private readonly ILogger<UserAuth> _logger;
//         private IConfiguration _config;

//         public UserAuth(ILogger<UserAuth> logger, IConfiguration config)
//         {
//             _logger = logger;
//             _config = config;
//         }

//         [HttpGet]
//         [Authorize]
//         public ActionResult<string> Get()
//         {
//             return Ok("Hello user, you are authenticated.");
//         }

//         [HttpGet("Auth")]
//         public ActionResult<string> GetUserAuth([FromQuery] string name)
//         {
//             var token = GenerateToken(name);

//             return Ok(token);
//         }

//         private string GenerateToken(string name)
//         {
//             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//             const string Name = nameof(Name);

//             var claims = new[]
//             {
//             new Claim(Name, name),
//         };

//             var token = new JwtSecurityToken(_config["Jwt:Issuer"],
//                 _config["Jwt:Audience"],
//                 claims,
//                 expires: DateTime.Now.AddMinutes(60 * 48),
//                 signingCredentials: credentials);

//             return new JwtSecurityTokenHandler().WriteToken(token);
//         }

//     }
// }
