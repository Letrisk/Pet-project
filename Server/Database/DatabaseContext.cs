namespace Server.Database
{
    using System.Data.Entity;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DBConnection")
        {
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<ClientEvent> EventLog { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }
    }
}
