namespace Common.Network
{
    public class Client
    {
        #region Properties

        public string Login { get; set; }

        public bool IsOnline { get; set; }

        #endregion Properties

        #region Constructors

        public Client(string login, bool isOnline)
        {
            Login = login;
            IsOnline = isOnline;
        }

        #endregion Constructors
    }
}
