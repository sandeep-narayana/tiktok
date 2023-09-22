
using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IAuthRepository
{
    Task<bool> CheckAuthentication(UserCredentials userCredentials);
    Task<bool> RegisterUser(User user, string otp);
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


    public async Task<bool> RegisterUser(User user, string otp)
    {

        using var con = NewConnection;
        // SQL query to cofim the otp with the username
        var otpConfirmationQuery = "SELECT * FROM otp_verification where email = @Email AND used = @Bool";
        var emailWithotp = await con.QueryAsync<OtpVerification>(otpConfirmationQuery, new
        {
            Email = user.Email,
            Bool = false

        });
        // this will return empty collection 
        if (emailWithotp.Count() == 0)
        {
            throw new Exception("No data fopund for otp verification");
        }

        // if confirm then mark it used and move forward
        var markedUsedQuery = "UPDATE otp_verification SET used = @True WHERE email = @UserEmail AND otp = @Otp";
        var isChanged = await con.ExecuteAsync(markedUsedQuery, new
        {
            UserEmail = user.Email,
            Otp = otp,
            True = true
        }) > 0;

        if (!isChanged)
        {
            throw new Exception("Something went wrong");
        }

        // SQL query to insert a new user into the database.
        var registrationQuery = "INSERT INTO users (first_name,last_name ,email, password) VALUES (@FirstName,@LastName,@Email, @Password)";

        // Hash the password before storing it in the database.
        var hashedPassword = HashPassword(user.Password);
        var parameters = new
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = hashedPassword
        };

        await con.ExecuteAsync(registrationQuery, parameters);
        return true;

    }

    private string HashPassword(string password)
    {
        // Generate a salt and hash the password with it using BCrypt.
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12); // You can adjust the salt work factor (12 in this example).
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    public class OtpVerification
    {

    }
}