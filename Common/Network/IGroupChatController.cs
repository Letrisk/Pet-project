namespace Common.Network
{
    using System.Collections.Generic;

    public interface IGroupChatController
    {
        #region Properties

        string Login { get; set; }

        #endregion Properties

        #region Methods

        void SendCreateGroupRequest(string groupName, List<string> clients);

        #endregion Methods
    }
}
