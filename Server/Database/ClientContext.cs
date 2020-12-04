namespace Server.Database
{
    using System.Data.Entity;

    public class ClientContext : DbContext
    {
        public ClientContext() : base("DbConnection")
        {
        }

        public DbSet<Client> Clients { get; set; }
    }
}
