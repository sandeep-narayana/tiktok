using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories
{
    public interface IOtpRepository
    {
        public Task<bool> saveOtpInDataBase(string otpString, string userEmail);
        public Task<bool> changeOtpUseStatusToTrue(string otpString, string userEmail);
    }

    public class OtpRepository : BaseRepository, IOtpRepository
    {
        public OtpRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> changeOtpUseStatusToTrue(string otpString, string userEmail)
        {
            var query = "UPDATE otp_verification SET used = @True WHERE email = @UserEmail AND otp = @Otp";
            var con = NewConnection;
            var res = await con.ExecuteAsync(query, new
            {
                UserEmail = userEmail,
                Otp = otpString,
                True = true
            }) > 0;
            return res;
        }

        public async Task<bool> saveOtpInDataBase(string otpString, string userEmail)
        {
            var query = "INSERT INTO otp_verification (email,otp,used) VALUES (@UserEmail,@Otp,@Used)";
            var con = NewConnection;
            var res = await con.ExecuteAsync(query, new
            {
                UserEmail = userEmail,
                Otp = otpString,
                Used = false
            }) > 0;

            return res;

        }
    }
}