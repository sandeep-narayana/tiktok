using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;
public interface IWishListRepository
{
    public Task<List<Product>> getWishListByUserID(int UserId);
}

public class WishListRepository : BaseRepository, IWishListRepository
{
    public WishListRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<List<Product>> getWishListByUserID(int UserId)
    {

        var query = "SELECT * FROM products where id IN (SELECT product_id from wishlist where user_id = @UserId)";

        var con = NewConnection;

        var wishlist = await con.QueryAsync<Product>(query, new { UserId = UserId });
        return wishlist.AsList();
    }
}

