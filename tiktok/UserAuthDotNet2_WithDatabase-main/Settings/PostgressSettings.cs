namespace UserAuthDotBet2_WithDatabase
{
    public class PostgressSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Database { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get => $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database};Application Name=Nconnect API Server;Include Error Detail=true"; }
    }
}