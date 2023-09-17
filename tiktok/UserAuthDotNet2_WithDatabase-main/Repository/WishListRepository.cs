using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;
public interface IWishListRepository
{
    public Task<List<Product>> getWishListByUserID(int UserId);

    public Task<bool> deleteProductById(int ProductId, int userId);

    public Task<string> AddToWishlist(Product product, int userId);
}

public class WishListRepository : BaseRepository, IWishListRepository
{
    public WishListRepository(IConfiguration config) : base(config)
    {

    }

    public Task<string> AddToWishlist(Product product, int userId)
    {
        
    }

    public async Task<bool> deleteProductById(int ProductId, int UserId)
    {
        var query = "DELETE FROM wishlist where product_id = @ProductId AND user_id = @UserID";
        var con = NewConnection;
        return await con.ExecuteAsync(query, new
        {
            ProductId = ProductId,
            UserID = UserId
        }) > 0;
    }

    public async Task<List<Product>> getWishListByUserID(int UserId)
    {

        var query = "SELECT * FROM products where id IN (SELECT product_id from wishlist where user_id = @UserId)";

        var con = NewConnection;

        var wishlist = await con.QueryAsync<Product>(query, new { UserId = UserId });
        return wishlist.AsList();
    }

    
}

