using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> get();
    public Task<String> AddCategory(CategoryDTO category);
}

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    public CategoryRepository(IConfiguration config) : base(config)
    {
    }

    async public Task<List<Category>> get()
    {
        var query = "SELECT * FROM categories";
        using var con = NewConnection;

        var res = await con.QueryAsync<Category>(query);
        return res.AsList();

    }

    async public Task<String> AddCategory(CategoryDTO category)
{
    var query = "INSERT INTO categories (name, description, image) VALUES (@Name, @Description, @Image)";
    using var con = NewConnection;

    var affectedRows = await con.ExecuteAsync(query, category);

    if (affectedRows > 0)
    {
        return "Category Added Successfully";
    }
    else
    {
        throw new Exception("Failed to insert category into the database.");
    }
}
}