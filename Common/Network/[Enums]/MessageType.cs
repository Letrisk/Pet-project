namespace Common.Network
{
    using System;

    [Flags]
    public enum MessageType
    {
        Event = 1,
        Error = 2
    }
}
