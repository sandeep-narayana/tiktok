

using Npgsql;
namespace UserAuthDotBet2_WithDatabase
{
    public class BaseRepository
    {
        protected readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        protected BaseRepository(IConfiguration config)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _configuration = config;
            this.ConnectionString = _configuration.GetSection(nameof(PostgressSettings)).Get<PostgressSettings>()!.ConnectionString;
        }




        public NpgsqlConnection NewConnection => new NpgsqlConnection(ConnectionString);

    }
}