namespace Common.Network.Messages
{
    using System.Collections.Generic;

    public class GroupsListResponse
    {
        #region Properties

        public Dictionary<string, List<string>> Groups { get; set; }

        #endregion Properties

        #region Constructors

        public GroupsListResponse(Dictionary<string, List<string>> groups)
        {
            Groups = groups;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GroupsListResponse),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
