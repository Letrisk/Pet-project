namespace Common.Network
{
    using System;

    using Messages;

    public class EventLogController : IEventLogController
    {
        #region Fields

        private readonly IController _controller;

        #endregion Fields

        #region Constructors

        public EventLogController(IController controller)
        { 
            _controller = controller;
        }

        #endregion Constructors

        #region Methods

        public void SendFilterRequest(DateTime firstDate, DateTime secondDate, MessageType messageTypes)
        {
            _controller.Send(new FilterRequest(firstDate, secondDate, messageTypes).GetContainer());
        }

        #endregion Methods
    }
}
