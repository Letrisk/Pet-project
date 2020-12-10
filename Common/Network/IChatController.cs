﻿namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;

    using WebSocketSharp;

    using Messages;

    public interface IChatController
    {
        #region Events

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;
        event EventHandler<FilteredMessagesReceivedEventArgs> FilteredMessagesReceived;
        event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;
        event EventHandler<GroupsReceivedEventArgs> GroupsReceived;

        #endregion Events

        #region Properties

        WebSocket Socket { get; set; }

        string Login { get; set; }

        #endregion Properties

        #region Methods

        void Disconnect();

        void Send(string source, string target, string message, string groupName = null);

        #endregion Methods
    }
}
