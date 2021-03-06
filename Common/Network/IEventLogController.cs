﻿namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;

    using WebSocketSharp;

    using Messages;

    public interface IEventLogController
    {
        #region Methods

        void SendFilterRequest(DateTime firstDate, DateTime secondDate, MessageType messageTypes);

        #endregion Methods
    }
}
