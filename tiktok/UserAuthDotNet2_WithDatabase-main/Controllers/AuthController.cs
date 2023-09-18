using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserAuthDotBet2_WithDatabase.Repositories;
using System.Text.Json.Serialization;

namespace UserAuthDotBet2_WithDatabase
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private IConfiguration _config;
        private IAuthRepository _auth;

        private IUserRepository _user;

        public AuthController(ILogger<AuthController> logger, IConfiguration config, IAuthRepository auth, IUserRepository user)
        {
            _logger = logger;
            _config = config;
            _auth = auth;
            _user = user;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] User user)
        {

            try
            {
                // Validate the registration request, check if the username or email is already taken, etc.
                // Perform registration logic, such as creating a new user in your database with hashed password.

                // For this example, let's assume you have a method in your _auth repository to handle registration.
                var registrationResult = await _auth.RegisterUser(user);

                if (registrationResult)
                {
                    // Registration successful, generate and return a JWT token.
                    // var token = GenerateToken(user.Email);
                    return Ok();
                }
                else
                {
                    // Registration failed, return an appropriate response.
                    return BadRequest("Registration failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, "Internal Server Error");
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserCredentials userCredentials)
        {
            try
            {

                var user = await _user.getUserByEmail(userCredentials.Email);

                if (user == null)
                {
                    // User not found, return a 404 response.
                    return NotFound("User not found");
                }


                var didAuthorize = await _auth.CheckAuthentication(userCredentials);

                if (!didAuthorize)
                {
                    // Unauthorized, return a 401 response.
                    return Unauthorized("Authentication failed");
                }

                // Authentication successful, generate and return a JWT token.
                var token = GenerateToken(userCredentials.Email, user.userid);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Welcome")]
        [Authorize]
        public async Task<ActionResult<string>> Welcome()
        {
            try
            {
                // Authorized user, return a welcome message.
                return Ok("Welcome, you are an authenticated user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the welcome request.");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("no_auth")]

        public ActionResult<string> NoAuth()
        {
            try
            {
                // Authorized user, return a welcome message.
                return Ok("Welcome user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the welcome request.");
                return StatusCode(500, "Internal Server Error");
            }
        }


        private string GenerateToken(string name, int id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            const string Name = nameof(Name);
            const string Id = nameof(Id);

            var claims = new List<Claim>
        {
            new Claim(Name, name),
            new Claim(Id, id.ToString()),
        };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60 * 48),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class User
        {
            [JsonPropertyName("userid")]
            public int userid { get; set; }
            public string Email { get; set; }

            [JsonPropertyName("first_name")] // Specify the JSON property name
            public string FirstName { get; set; }
            [JsonPropertyName("second_name")] // Specify the JSON property name
            public string LastName { get; set; }
            public string Password { get; set; }
        }

        public class UserCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Orders
        {
            public int Id { get; set; }
            [JsonPropertyName("order_id")]

            public string OrderId { get; set; }

            [JsonPropertyName("product_id")]
            public int ProductId { get; set; }

            public int Quantity { get; set; }

            [JsonPropertyName("order_price")]
            public decimal OrderPrice { get; set; }

            [JsonPropertyName("order_date")]
            public DateTime OrderDate { get; set; }

            [JsonPropertyName("user_id")]
            public int UserId { get; set; }

            public List<Product> products { get; set; }

            [JsonPropertyName("payment_type_id")]

            public int PaymentTypeId { get; set; }

        }

    }
}