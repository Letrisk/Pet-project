namespace Server.Database
{
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        [Key]
        public string Login { get; set; }
    }
}
