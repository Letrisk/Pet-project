namespace Server.Database
{
    using System.Data.Entity;

    public class ClientEventContext : DbContext
    {
        public ClientEventContext() : base("DbConnection")
        {
        }

        public DbSet<ClientEvent> EventLog { get; set; }
    }
}
