
using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IAuthRepository
{
    Task<bool> CheckAuthentication(UserCredentials userCredentials);
    Task<bool> RegisterUser(User user);
}

public class AuthRepository : BaseRepository, IAuthRepository
{
    public AuthRepository(IConfiguration config) : base(config)
    {
    }

    public async Task<bool> CheckAuthentication(UserCredentials userCredentials)
    {
        var query = "SELECT password FROM users WHERE email = @Email";

        var parameters = new
        {
            Email = userCredentials.Email
        };

        using var con = NewConnection;
        var hashedPasswordFromDb = await con.QueryFirstOrDefaultAsync<string>(query, parameters);

        if (hashedPasswordFromDb != null)
        {
            // Use BCrypt to verify if the provided plaintext password matches the stored hashed password.
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userCredentials.Password, hashedPasswordFromDb);

            return isPasswordValid;
        }

        // Authentication failed.
        return false;
    }


    public async Task<bool> RegisterUser(User user)
    {
        // SQL query to insert a new user into the database.
        var query = "INSERT INTO users (first_name,last_name ,email, password) VALUES (@FirstName,@LastName,@Email, @Password)";

        // Hash the password before storing it in the database.
        var hashedPassword = HashPassword(user.Password);

        var parameters = new
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = hashedPassword
        };

        using var con = NewConnection;
        await con.ExecuteAsync(query, parameters);
        return true;

    }

    private string HashPassword(string password)
    {
        // Generate a salt and hash the password with it using BCrypt.
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12); // You can adjust the salt work factor (12 in this example).
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }
}