using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IProductRepository
{
    public Task<List<Product>> get();
    public Task<Product> getProductById(int productId);
    public Task<bool> updateProduct(Product product);

    public Task<bool> deleteProductById(int productId);


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

    async public Task<Product> getProductById(int productId)
    {
        var query = "SELECT * FROM products WHERE id = @Id";
        var con = NewConnection;
        var res = await con.QuerySingleOrDefaultAsync<Product>(query, new
        {
            Id = productId
        });
        return res;
    }

    public async Task<bool> updateProduct(Product product)
    {
        var query = "UPDATE PRODUCTS SET name = @Name , description = @Description , image = @Image, price = @Price, category_id = @CategoryId WHERE id =@Id";
        var con = NewConnection;

        var affectedRows = await con.ExecuteAsync(query, new
        {
            Name = product.Name,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price,
            CategoryId = product.CategoryId,
            Id = product.Id
        });
        return affectedRows > 0;  // Return true if at least one row was updated
    }

    public async Task<bool> deleteProductById(int productId)
    {
        var query = "DELETE FROM products WHERE id = @ProductId ";
        var con = NewConnection;
        var affectedRow = await con.ExecuteAsync(query, new
        {
            ProductId = productId
        });

        return affectedRow > 0;
    }
}