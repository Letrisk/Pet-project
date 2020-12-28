namespace Common.Network
{
    using System.Collections.Generic;

    public class GroupsReceivedEventArgs
    {
        #region Properties

        public Dictionary<string, List<string>> Groups { get; }

        #endregion Properties

        #region Constructors

        public GroupsReceivedEventArgs(Dictionary<string, List<string>> groups)
        {
            Groups = groups;
        }

        #endregion Constructors
    }
}
