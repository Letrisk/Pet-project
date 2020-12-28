namespace Common.Network
{
    using System.Collections.Generic;

    using Messages;

    public class GroupChatController : IGroupChatController
    {
        #region Fields

        private readonly IController _controller;

        #endregion Fields

        #region Constructors

        public GroupChatController(IController controller)
        {
            _controller = controller;
        }

        #endregion Constructors

        #region Methods

        public void CreateGroupRequest(string groupName, List<string> clients)
        {
            _controller.Send(new CreateGroupRequest(groupName, clients).GetContainer());
        }

        #endregion Methods
    }
}
