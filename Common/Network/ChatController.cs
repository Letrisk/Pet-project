namespace Common.Network
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;

    using WebSocketSharp;

    using Messages;
    using Network;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ChatController : IChatController
    {
        #region Fields

        private readonly IController _controller;

        #endregion Fields

        #region Properties

        public string Login { get; set; }

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;
        public event EventHandler<FilteredMessagesReceivedEventArgs> FilteredMessagesReceived;
        public event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;
        public event EventHandler<GroupsReceivedEventArgs> GroupsReceived;

        #endregion Events

        #region Constructors

        public ChatController(IController controller)
        {
            _controller = controller;
            _controller.ConnectionStateChanged += HandleConnectionStateChanged;
            _controller.ConnectionReceived += HandleConnectionReceived;
            _controller.MessageReceived += HandleMessageReceived;
            _controller.ClientsListReceived += HandleClientListReceived;
            _controller.ChatHistoryReceived += HandleChatHistoryReceived;
            _controller.FilteredMessagesReceived += HandleFilteredMessagesReceived;
            _controller.GroupsReceived += HandleGroupsListReceived;
        }

        #endregion Constructors

        #region Methods

        public void Disconnect()
        {
            Login = String.Empty;
            _controller.Disconnect();
        }

        public void Send(string target, string message, string groupName)
        {
            if(target == "General")
            {
                target = String.Empty;
            }

            _controller.Send(new MessageRequest(target, message, groupName).GetContainer());
        }

        public void LeaveGroup(string source, string groupName)
        {
            _controller.Send(new LeaveGroupRequest(groupName).GetContainer());
        }

        public void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Client))
            {
                ConnectionStateChanged?.Invoke(this, e);
            }
        }

        public void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            ConnectionReceived?.Invoke(this, e);
        }

        public void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public void HandleClientListReceived(object sender, ClientsListReceivedEventArgs e)
        {
            ClientsListReceived?.Invoke(this, e);
        }

        public void HandleChatHistoryReceived(object sender, ChatHistoryReceivedEventArgs e)
        {
            ChatHistoryReceived?.Invoke(this, e);
        }

        public void HandleFilteredMessagesReceived(object sender, FilteredMessagesReceivedEventArgs e)
        {
            FilteredMessagesReceived?.Invoke(this, e);
        }

        public void HandleGroupsListReceived(object sender, GroupsReceivedEventArgs e)
        {
            GroupsReceived?.Invoke(this, e);
        }

        #endregion Methods
    }
}
