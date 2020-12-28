namespace Server_WinService
{
    using System;
    using System.ServiceProcess;
    using System.Threading;

    using Server;

    public partial class ServiceServer : ServiceBase
    {
        private NetworkManager _networkManager;

        public ServiceServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _networkManager = new NetworkManager();
            _networkManager.Start();
        }

        protected override void OnStop()
        {
            _networkManager.Stop();
        }

        internal void TetsStartAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
