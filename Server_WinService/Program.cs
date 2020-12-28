namespace Server_WinService
{
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*if (Environment.UserInteractive)
            {
                ServiceServer service = new ServiceServer();
                service.TetsStartAndStop(args);
            }
            else
            {*/
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ServiceServer()
                };
                ServiceBase.Run(ServicesToRun);
            //}
        }
    }
}
