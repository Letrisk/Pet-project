namespace Common.Network.Messages
{
    using System.Collections.Generic;

    public class GroupBroadcast
    {
        #region Properties

        public Dictionary<string, List<string>> Group { get; set; }

        #endregion Properties

        #region Constructors

        public GroupBroadcast(Dictionary<string, List<string>> group)
        {
            Group = group;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GroupBroadcast),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
