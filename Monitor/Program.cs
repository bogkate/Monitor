using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    using System.Net;
    using System.ServiceModel;
    using DataContract;

    class Program
    {
        static void Main(string[] args)
        {
            IPAddress address;
            int ipPort;
            string ipPortStr;
            string addressStr;
            do
            {
                Console.WriteLine("Enter ip-address:");
                addressStr = Console.ReadLine();
            }
            while (!IPAddress.TryParse(addressStr, out address));

            do
            {
                Console.WriteLine("Enter ip-port:");
                ipPortStr = Console.ReadLine();
            }
            while (!int.TryParse(ipPortStr, out ipPort));

            try
            {
                string urlService = $"net.tcp://{address}:{ipPort}";
                var tcpBinding = new NetTcpBinding { TransactionFlow = false, ReceiveTimeout = TimeSpan.MaxValue };
                tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
                tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                tcpBinding.Security.Mode = SecurityMode.None;

                var servioServer = new TestServer();
                var serviceHost = new ServiceHost(servioServer);
                serviceHost.AddServiceEndpoint(typeof(IServer), tcpBinding, urlService);
                serviceHost.Open();

                Console.WriteLine("Enter to stop server...");
                Console.ReadLine();

                serviceHost.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Start service error! {exception}");
            }
        }
    }
}
