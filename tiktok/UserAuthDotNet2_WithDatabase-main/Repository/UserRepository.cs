using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IUserRepository
{
    public Task<User> getUserByEmail(string email);
    public Task<User> getUser(int id);
    public Task<List<User>> getusers();

    public Task<bool> updateUser(User user);

    public Task<bool> deleteUserById(int userId);

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

    public async Task<List<User>> getusers()
    {
        var query = "SELECT * FROM users";
        var con = NewConnection;
        var users = await con.QueryAsync<User>(query);
        return users.ToList();
    }

    public async Task<bool> updateUser(User user)
    {
        var query = "UPDATE users SET email = @Email, first_name = @FirstName ,last_name = @LastName , image = @Image, role = @Role WHERE user_id =@UserId";
        var con = NewConnection;

        var affectedRows = await con.ExecuteAsync(query, new
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Image = user.Image,
            Role = user.Role,
            UserId = user.userid
        });
        return affectedRows > 0;  // Return true if at least one row was updated
    }

    public async Task<bool> deleteUserById(int userId)
    {
        var query = "DELETE FROM users WHERE user_id = @UserId ";
        var con = NewConnection;
        var affectedRow = await con.ExecuteAsync(query, new
        {
            UserId = userId
        });

        return affectedRow > 0;
    }
}