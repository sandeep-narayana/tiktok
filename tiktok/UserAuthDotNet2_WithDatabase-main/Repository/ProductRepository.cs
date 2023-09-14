using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IProductRepository
{
    public Task<List<Product>> get();
    public Task<Product> getProductById(int productId);
}

public class ProductRepository : BaseRepository, IProductRepository
{
    public ProductRepository(IConfiguration config) : base(config)
    {
    }

    async public Task<List<Product>> get()
    {
        var query = "SELECT * FROM products";
        using var con = NewConnection;

        var res = await con.QueryAsync<Product>(query);
        return res.AsList();

    }

    async public  Task<Product> getProductById(int productId)
    {
        var query = "SELECT * FROM products WHERE id = @Id";
        var con = NewConnection;
        var res = await con.QuerySingleOrDefaultAsync<Product>(query, new
        {
            Id = productId
        });
        return res;
    }
}