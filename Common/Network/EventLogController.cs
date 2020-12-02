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

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private WebSocket _socket;

        private int _sending;
        private string _login;

        #endregion Fields

        #region Properties

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        public WebSocket Socket
        {
            get => _socket;
            set
            {
                _socket = value;
            }
        }

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

        public EventLogController()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _sending = 0;
        }

        #endregion Constructors

        #region Methods

        public void SendFilterRequest(DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {
            _sendQueue.Enqueue(new FilterRequest(_login, firstDate, secondDate, messageTypes).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                SendImpl();
        }

        private void SendCompleted(bool completed)
        {
            /*if (!completed)
            {
                Disconnect();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, false));
                return;
            }*/

            SendImpl();
        }

        private void SendImpl()
        {
            if (!IsConnected)
                return;

            if (!_sendQueue.TryDequeue(out var message) && Interlocked.CompareExchange(ref _sending, 0, 1) == 1)
                return;

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        #endregion Methods
    }
}
