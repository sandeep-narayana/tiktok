
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

        private IWishListRepository _wishList;

        private IOrderRepository _orders;

        public ProjectController(ILogger<ProjectController> logger, ICategoryRepository cat, IProductRepository product, ICartRepository cart, IUserRepository user, IWishListRepository wishList, IOrderRepository orders)
        {
            _logger = logger;
            _cat = cat;
            _product = product;
            _cart = cart;
            _user = user;
            _wishList = wishList;
            _orders = orders;
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
        [Authorize]
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

        [HttpDelete("cart")]
        [Authorize]

        public async Task<ActionResult<bool>> DeleteproductFromCart([FromQuery] int productId)
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
            var response = await _cart.deleteProductById(productId, userId);
            // Return the user data
            return Ok(response);
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

        [HttpGet("wishlist")]
        [Authorize]
        public async Task<ActionResult<Product>> GetWishList()
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
            var wishList = await _wishList.getWishListByUserID(userId);

            // Return the user data
            return Ok(wishList);
        }

        [HttpDelete("wishlist")]
        [Authorize]

        public async Task<ActionResult<bool>> DeleteproductFromWishlist([FromQuery] int productId)
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
            var response = await _wishList.deleteProductById(productId, userId);
            // Return the user data
            return Ok(response);
        }

        [HttpPost("wishlist/add")]
        [Authorize]
        public async Task<ActionResult<string>> AddtoWishList([FromQuery] int productId)
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


                var result = await _wishList.AddToWishlist(product.Id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while adding the product to the wishlist.");
            }
        }


        [HttpGet("orders")]
        [Authorize]
        public async Task<ActionResult<Product>> GetAllOrders()
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
            var wishList = await _orders.GetOrders(userId);

            // Return the user data
            return Ok(wishList);
        }

        [HttpPost("orders")]
        [Authorize]
        public async Task<ActionResult<String>> PlaceOrders([FromQuery] int PaymentTypeId)
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

            // use the userid to fetch the cart item (only selected ones)(selected one to be imp in future)
            var cart = await _cart.getCartById(userId);

            if (cart.Count == 0)
            {
                throw new Exception("No selected item found in cart");
            }
            // Use the user ID to fetch the user data
            var orderPlace = await _orders.placeOrders(cart, userId, PaymentTypeId);

            // Return the user data
            return Ok(orderPlace);
        }

        [HttpPut("change_quantity")]
        [Authorize]
        public async Task<ActionResult<bool>> ChangeQuantity([FromQuery] int cartId, int newQuantity)
        {

            if (newQuantity == 0)
            {
                throw new Exception("Cant set a value 0");
            }
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

            // use the userid to fetch the cart item (only selected ones)(selected one to be imp in future)
            var cart = await _cart.getCartById(userId);

            if (cart.Count == 0)
            {
                throw new Exception("No selected item found in cart");
            }

            var quantityChanged = await _cart.changeQuantity(cartId, newQuantity, userId);
            return Ok(quantityChanged);

        }

        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                // Check whether the user has the permission or not
                // If yes, then get the result
                var users = await _user.getusers();

                return Ok(users); // Return a 200 OK response with the list of users
            }
            catch (Exception ex)
            {
                // Handle exceptions here and return an appropriate error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // instead of this make a new deactivate product because the product is in the orders also
        [HttpDelete("product")]
        [Authorize]
        public async Task<ActionResult<bool>> Deleteproduct([FromQuery] int productId)
        {
            try
            {
                var product = await _product.getProductById(productId);
                if (product == null)
                {
                    throw new Exception("No product found with this product id");
                }
                return await _product.deleteProductById(productId);

            }
            catch (Exception ex)
            {
                // Handle exceptions here and return an appropriate error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("product")]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] Product product)
        {
            try
            {
                var existed = await _product.getProductById(product.Id);
                if (existed == null)
                {
                    throw new Exception("No product found with this product id");
                }
                return await _product.updateProduct(product);

            }
            catch (Exception ex)
            {
                // Handle exceptions here and return an appropriate error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; } // Assuming you have a category ID for the product
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        [JsonPropertyName("category_id")]
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
        public int Quantity { get; set; }

        [JsonPropertyName("product_id")]
        public int ProductId { get; set; }

    }
}