namespace Server.Database
{
    using System.Data.Entity;

    public class DatabaseContext : DbContext
    {
        #region Properties

        public DbSet<Client> Clients { get; set; }

        public DbSet<ClientEvent> EventLog { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }

        #endregion Properties

        #region Constructors

        public DatabaseContext(string connectionString) : base("DBConnection")
        {
            Database.Connection.ConnectionString = connectionString;
        }

        #endregion Constructors
    }
}
