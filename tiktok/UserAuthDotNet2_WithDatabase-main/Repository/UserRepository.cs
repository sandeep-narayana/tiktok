using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IUserRepository
{
    public Task<User> getUserByEmail(string email);
     public Task<User> getUser(int id);

}

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration config) : base(config)
    {
    }

    async public Task<User> getUserByEmail(string email)
    {

        var query = "SELECT * FROM users WHERE email = @Email";
        var con = NewConnection;
        var res = await con.QueryFirstOrDefaultAsync<User>(query, new
        {
            Email = email
        });

        return res;
    }

    async public Task<User> getUser(int userId)
    {

        var query = "SELECT * FROM users WHERE user_id = @UserId";
        var con = NewConnection;
        var res = await con.QueryFirstOrDefaultAsync<User>(query, new
        {
            UserId = userId
        });

        return res;
    }

}