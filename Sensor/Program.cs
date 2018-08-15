using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataContract;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sensor
{

    class Program
    {
        private static IServer wcfService;
        private static EndpointAddress endpointAddress;
        private static NetTcpBinding tcpBinding;
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

            string urlService = $"net.tcp://{address}:{ipPort}";
            endpointAddress = new EndpointAddress(urlService);

            tcpBinding = new NetTcpBinding
            {
                OpenTimeout = TimeSpan.FromSeconds(15),
                SendTimeout = TimeSpan.FromSeconds(15),
                ReceiveTimeout = TimeSpan.MaxValue,
                TransactionFlow = false
            };
            tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            tcpBinding.Security.Mode = SecurityMode.None;

            var rand = new Random();
            while (true)
            {
                try
                {
                    CheckChannel();

                    wcfService.SetMetric(rand.Next(int.MinValue, int.MaxValue));

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                Task.Delay(new TimeSpan(0, 0, 0, 3)).Wait();
            }
        }

        private static bool CheckChannel()
        {
            bool result;

            try
            {
                if (wcfService is IChannel)
                {
                    var channel = wcfService as IChannel;
                    if (channel.State == CommunicationState.Faulted)
                    {
                        Console.WriteLine("Channel in FAIL state! Aborting...");

                        channel.Abort();
                        wcfService = null;
                    }
                }
                else
                {
                    wcfService = null;
                }


                if (wcfService == null)
                {
                    wcfService = ChannelFactory<IServer>.CreateChannel(tcpBinding, endpointAddress);
                    Console.WriteLine("New Channel created!");
                }

                result = true;
            }
            catch (Exception exception)
            {
                result = false;
                Console.WriteLine($"CheckChannel error {exception}");
            }

            return result;
        }
    }
}
