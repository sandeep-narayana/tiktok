using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface ICartRepository
{
    public Task<List<Cart>> getCartById(int UserId);
    public Task<string> AddToCart(Product product, int userId);
}

public class CartRepository : BaseRepository, ICartRepository
{
    public CartRepository(IConfiguration config) : base(config)
    {
    }


    public async Task<List<Cart>> getCartById(int UserId)
    {
        var query = "SELECT * FROM cart where user_id = @UserId";
        using var con = NewConnection;

        var res = await con.QueryAsync<Cart>(query, new { UserId = UserId });
        return res.AsList();
    }

    public async Task<string> AddToCart(Product product, int userId)
    {
        const string query = "INSERT INTO cart (user_id, name, description, image, price, quantity) VALUES (@UserId, @Name, @Description, @Image, @Price, @Quantity)";

        using var con = NewConnection;

        var res = await con.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = product.Name,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price,
            Quantity = 1 // You can set the quantity to 1 when adding a single product to the cart
        });

        if (res > 0)
        {
            return "Product added to cart successfully";
        }
        else
        {
            return "Failed to add the product to the cart";
        }
    }
}