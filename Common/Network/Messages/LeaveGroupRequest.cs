namespace Common.Network.Messages
{
    public class LeaveGroupRequest
    {
        #region Properties

        public string GroupName { get; set; }

        #endregion Properties

        #region Constructors

        public LeaveGroupRequest(string groupName)
        {
            GroupName = groupName;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(LeaveGroupRequest),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
