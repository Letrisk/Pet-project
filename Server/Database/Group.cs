namespace Server.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Group
    {
        [Key]
        public string GroupName { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public Group()
        {
            Clients = new List<Client>();
        }
    }
}
