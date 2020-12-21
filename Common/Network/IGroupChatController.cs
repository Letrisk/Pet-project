namespace Common.Network
{
    using System.Collections.Generic;

    public interface IGroupChatController
    {
        #region Methods

        void CreateGroupRequest(string groupName, List<string> clients);

        #endregion Methods
    }
}
