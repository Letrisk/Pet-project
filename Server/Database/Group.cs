namespace Server.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Group
    {
        #region Properties

        [Key]
        public string GroupName { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        #endregion Properties

        #region Constructors

        public Group()
        {
            Clients = new List<Client>();
        }

        #endregion Constructors
    }
}
