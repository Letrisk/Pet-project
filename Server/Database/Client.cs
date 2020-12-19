namespace Server.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Client
    {
        #region Properties

        [Key]
        public string Login { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        #endregion Properties

        #region Constructors

        public Client()
        {
            Groups = new List<Group>();
        }

        #endregion Constructors
    }
}
