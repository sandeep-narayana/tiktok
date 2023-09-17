using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IOrderRepository
{
    public Task<List<Orders>> GetOrders(int UserId);
    public Task<List<String>> PlaceOrders();
}


public class OrderRepository : BaseRepository, IOrderRepository
{
    public OrderRepository(IConfiguration config) : base(config)
    {
    }

    public async Task<List<Orders>> GetOrders(int UserId)
    {
        var query = "SELECT * FROM orders WHERE user_id = @UserId";
        var con = NewConnection;
        var res = await con.QueryAsync<Orders>(query, new
        {
            UserId = UserId
        });
        return res.AsList();
        throw new NotImplementedException();
    }

    public Task<List<string>> PlaceOrders()
    {
        throw new NotImplementedException();
    }
}
