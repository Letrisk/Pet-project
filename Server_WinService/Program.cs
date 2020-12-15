using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Server_WinService
{
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
