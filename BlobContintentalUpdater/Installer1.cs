using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace BlobContinentalUpdater
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
            si.ServiceName = "BlobContinentalUpdaterService";
            si.Description =
@"Haalt blobs uit yyyy/MM/dd directory en stuurt de tirePressure data naar continental";
            si.StartType = ServiceStartMode.Manual;
            Installers.AddRange(new Installer[] { spi, si });
        }
    }
}
