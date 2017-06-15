using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KeyCodeReceiver
{
   public  class KeyReceiver
    {
        private string ip;
        private Keyboarder keyboarder;

        public KeyReceiver()
        {
            ip = GetIPAddress();
            keyboarder = new Keyboarder();
        }

        public void Run(int port)
        {
            Task.Run(() =>
            {
                Console.WriteLine("run thread");
                var localEP = new IPEndPoint(IPAddress.Parse(ip), port);
                var client = new UdpClient(localEP);

                while (true)
                {
                    IPEndPoint remoteEP = null;
                    var receiveBytes = client.Receive(ref remoteEP);

                    string receiveMsg = Encoding.UTF8.GetString(receiveBytes);

                    var values = receiveMsg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    keyboarder.InputKeys(values.ToList());
                }

                client.Close();
                Console.WriteLine("thread closed");
            });
            Console.WriteLine("run finish");
        }

        public static string GetIPAddress()
        {
            var ipaddress = "";
            IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }
            return ipaddress;
        }
    }
}
