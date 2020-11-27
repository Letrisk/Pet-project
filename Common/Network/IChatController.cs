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

        #endregion Events

        #region Properties

        WebSocket Socket { get; set; }

        ConcurrentQueue<MessageContainer> SendQueue { get; }

        //ObservableCollection<string> ClientsList { get; set; }

        int Sending { get; set; }

        string Login { get; set; }

        bool IsEnable { get; set; }

        #endregion Properties

        #region Methods

        void Disconnect();

        void Send(string source, string target, string message);

        #endregion Methods
    }
}
