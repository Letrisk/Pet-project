namespace Common.Network
{
    using System;

    using Common.Network._Enums_;

    public static class ControllerFactory
    {
        public static IController Create(TransportType type)
        {
            switch (type)
            {
                case TransportType.WebSocket:
                    return new WsController();
                /*case TransportType.Tcp:
                    return new TcpClient();*/
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
