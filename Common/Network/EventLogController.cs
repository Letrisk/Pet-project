namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Threading;

    using WebSocketSharp;

    using Messages;
    using Network;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class EventLogController : IEventLogController
    {
        #region Fields

        private readonly IController _controller;

        private string _login;

        #endregion Fields

        #region Properties

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
            }
        }

        #endregion Properties

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
