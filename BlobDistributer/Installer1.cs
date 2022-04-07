using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace BlobDistributer
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();

            ServiceProcessInstaller spi = new ServiceProcessInstaller();
            spi.Account = ServiceAccount.LocalSystem;

            ServiceInstaller si = new ServiceInstaller();
            si.ServiceName = "BlobDistributerService";
            si.Description =
@"Haalt blobs uit https://jupitertstexternal.blob.core.windows.net of https://jupiterprdexternal.blob.core.windows.net
en stopt ze in een yyyy/MM/dd directory structuur";
            si.StartType = ServiceStartMode.Automatic;
            Installers.AddRange(new Installer[] { spi, si });
        }
    }
}
