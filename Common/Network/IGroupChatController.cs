namespace Common.Network
{
    using System.Collections.Generic;

    public interface IGroupChatController
    {
        #region Methods

        void SendCreateGroupRequest(string groupName, List<string> clients);

        #endregion Methods
    }
}
