
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthDotBet2_WithDatabase.Repositories;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase
{

    [ApiController]
    [Route("api/categories")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private ICategoryRepository _cat;
        private IProductRepository _product;
        private ICartRepository _cart;

        private IUserRepository _user;

        public ProjectController(ILogger<ProjectController> logger, ICategoryRepository cat, IProductRepository product, ICartRepository cart, IUserRepository user)
        {
            _logger = logger;
            _cat = cat;
            _product = product;
            _cart = cart;
            _user = user;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {

            var categories = await _cat.get();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<String>> CreateCategory([FromBody] CategoryDTO newCategory)
        {
            if (newCategory == null)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                var res = await _cat.AddCategory(newCategory);
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _product.get();
            return Ok(products);
        }

        [HttpGet("cart")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetUserCart()
        {

            var userClaims = HttpContext.User.Claims;
            // Find the claim with the user's ID:
            var userIdClaim = userClaims.FirstOrDefault(claim => claim.Type == "Id");

            // Extract the user's ID as an integer:
            int userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId) ? parsedUserId : -1; // Default value if the claim is not found or parsing fails

            //Throw an exception if the user ID is -1
            if (userId == -1)
            {
                throw new Exception("User ID not found or invalid.");
            }

            var cart = await _cart.getCartById(userId);

            if (cart == null || !cart.Any())
            {
                return NoContent(); // Return 204 No Content if the cart is empty or doesn't exist.
            }
            return Ok(cart);
        }

        [HttpPost("cart/add")]
        [Authorize]
        public async Task<ActionResult<string>> AddProductToCart([FromQuery] int productId)
        {
            try
            {

                var userClaims = HttpContext.User.Claims;
                // Find the claim with the user's ID:
                var userIdClaim = userClaims.FirstOrDefault(claim => claim.Type == "Id");
                // Extract the user's ID as an integer:
                int userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId) ? parsedUserId : -1; // Default value if the claim is not found or parsing fails

                //Throw an exception if the user ID is -1
                if (userId == -1)
                {
                    throw new Exception("User ID not found or invalid.");
                }

                // find product with id
                var product = await _product.getProductById(productId);

                if (product == null)
                {
                    throw new Exception("Product not found.");
                }


                var result = await _cart.AddToCart(product, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while adding the product to the cart.");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser()
        {

            var userClaims = HttpContext.User.Claims;

            // Find the claim with the user's ID:
            var userIdClaim = userClaims.FirstOrDefault(claim => claim.Type == "Id");

            // Extract the user's ID as an integer:
            int userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId) ? parsedUserId : -1; // Default value if the claim is not found or parsing fails

            //Throw an exception if the user ID is -1
            if (userId == -1)
            {
                throw new Exception("User ID not found or invalid.");
            }

            // Use the user ID to fetch the user data
            var user = await _user.getUser(userId);

            // Return the user data
            return Ok(user);
        }


    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class CategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; } // Assuming you have a category ID for the product
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } // Assuming you have a category ID for the product
    }


    public class Cart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; } // Added userId property
    }

}