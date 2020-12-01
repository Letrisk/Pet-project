namespace Server.Database
{
    using System.Data.Entity;

    public class MessageContext : DbContext
    {
        public MessageContext() : base("DbConnection")
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
