namespace Server.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Client
    {
        [Key]
        public string Login { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public Client()
        {
            Groups = new List<Group>();
        }
    }
}
