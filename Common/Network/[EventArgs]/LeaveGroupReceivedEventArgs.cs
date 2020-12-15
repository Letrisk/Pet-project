namespace Common.Network
{
    public class LeaveGroupReceivedEventArgs
    {
        #region Properties

        public string Source { get; }

        public string GroupName { get; }

        #endregion Properties

        #region Constructors

        public LeaveGroupReceivedEventArgs(string source, string groupName)
        {
            Source = source;
            GroupName = groupName;
        }

        #endregion Constructors
    }
}
