using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace Server_WinService
{
    [RunInstaller(true)]
    public partial class ServerInstaller : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public ServerInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.StartType = ServiceStartMode.Manual;

            serviceInstaller.ServiceName = "ServiceServer";

            serviceInstaller.DisplayName = "Служба Сервер мессенджера";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
