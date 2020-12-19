namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;

    using WebSocketSharp;

    using Messages;

    public interface IController
    {
        #region Properties

       string Login { get; set; }

        #endregion Properties

        #region Events

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        event EventHandler<ErrorReceivedEventArgs> ErrorReceived;
        event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;
        event EventHandler<FilteredMessagesReceivedEventArgs> FilteredMessagesReceived;
        event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;
        event EventHandler<GroupsReceivedEventArgs> GroupsReceived;

        #endregion Events

        #region Methods

        void Connect(string address, string port);

        void Send(MessageContainer messageContainer);

        void Disconnect();

        #endregion Methods
    }
}
