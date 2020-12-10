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

        WebSocket Socket { get; set; }

        #endregion Properties

        #region Methods

        void Send(MessageContainer messageContainer);

        void Disconnect();

        #endregion Methods
    }
}
