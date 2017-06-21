using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KeyCodeReceiver
{
    public class KeyReceiver
    {
        private string ip;
        private Keyboarder keyboarder;
        private byte[] key;

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

                    if (key == null)
                    {
                        GetAesKey(remoteEP.Address.ToString(), port);
                    }

                    var clientMsg = Encoding.UTF8.GetString(receiveBytes);
                    var splitted = clientMsg.Split(new string[] { "?" }, StringSplitOptions.None);
                    var iv = Convert.FromBase64String(splitted[0]);
                    var encrypted = Convert.FromBase64String(splitted[1]);

                    string plain;
                    try
                    {
                        plain = Decrypt(iv, encrypted);
                    }
                    catch (CryptographicException)
                    {
                        //キーが変更されている
                        GetAesKey(remoteEP.Address.ToString(), port);
                        plain = Decrypt(iv, encrypted);
                    }
                    var keyCodes = plain.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    keyboarder.InputKeys(keyCodes.ToList());
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

        private void GetAesKey(string remoteIp, int port)
        {
            Console.WriteLine("start exchanging key");
            using (var tcpClient = new TcpClient(remoteIp, port))
            using (var netStream = tcpClient.GetStream())
            using (var reader = new StreamReader(netStream))
            using (var writer = new StreamWriter(netStream))
            {
                writer.WriteLine("connect");
                writer.Flush();

                string clientMsg;
                while ((clientMsg = reader.ReadLine()) != null)
                {
                    Console.WriteLine(clientMsg);
                    if (clientMsg != "ok")
                    {
                        return;
                    }
                }
            }

            var ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start();

            var rsa = new RSACryptoServiceProvider(2048);
            var cert = CreateSelfSignedCertificate("", rsa, "10000", DateTime.Now, DateTime.Now.AddDays(20));
            var certData = cert.Export(X509ContentType.Cert);

            using (var tcpClient = tcpListener.AcceptTcpClient())
            using (var netStream = tcpClient.GetStream())
            using (var streamReader = new StreamReader(netStream, Encoding.UTF8))
            {
                var clientMsg = streamReader.ReadLine();
                if (clientMsg != "connect")
                {
                    return;
                }

                var sendMsg = Encoding.UTF8.GetBytes(Convert.ToBase64String(certData, Base64FormattingOptions.None));
                netStream.Write(sendMsg, 0, sendMsg.Length);
                var terminate = Encoding.UTF8.GetBytes("\r\n");
                netStream.Write(terminate, 0, terminate.Length);
                netStream.Flush();
            }

            using (var tcpClient = tcpListener.AcceptTcpClient())
            using (var netStream = tcpClient.GetStream())
            using (var streamReader = new StreamReader(netStream, Encoding.UTF8))
            using (var writer = new StreamWriter(netStream, Encoding.UTF8))
            {
                var clientMsg = streamReader.ReadLine();
                byte[] enqrypted = Convert.FromBase64String(clientMsg);
                key = rsa.Decrypt(enqrypted, false);

                writer.WriteLine("ok");
                writer.Flush();
            }

            tcpListener.Stop();
            Console.WriteLine("succeeded in exchanging key");
        }

        private string Decrypt(byte[] iv, byte[] encrypted)
        {
            var csp = new AesCryptoServiceProvider();
            csp.BlockSize = 128;
            csp.KeySize = 256;
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            csp.IV = iv;
            csp.Key = key;

            using (var inms = new MemoryStream(encrypted))
            using (var decryptor = csp.CreateDecryptor())
            using (var cs = new CryptoStream(inms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private X509Certificate2 CreateSelfSignedCertificate(string issuerDN, RSA privateKey, string serialString, DateTime notBefore, DateTime notAfter)
        {
            // 証明書の属性
            var attr = new Dictionary<DerObjectIdentifier, string>()
            {
                { X509Name.CN, "link.studio-ephyra" },
                { X509Name.C, "Japan" },
                { X509Name.ST, "Nagano-Ken" },
                { X509Name.L, "Nagano-Shi" },
                { X509Name.O, "Studio-Ephyra" },
                { X509Name.OU, "None" },
            };
            var ord = new List<DerObjectIdentifier>()
            {
                X509Name.CN,
                X509Name.C,
                X509Name.ST,
                X509Name.L,
                X509Name.O,
                X509Name.OU,
            };

            // 証明書の生成
            var name = new X509Name(ord, attr);
            var x509gen = new X509V3CertificateGenerator();
            var serial = new BigInteger(serialString);
            var keyPair = DotNetUtilities.GetKeyPair(privateKey);
            x509gen.SetSerialNumber(serial);
            x509gen.SetIssuerDN(name);
            x509gen.SetSubjectDN(name);
            x509gen.SetNotBefore(notBefore);
            x509gen.SetNotAfter(notAfter);
            x509gen.SetPublicKey(keyPair.Public);
            // SHA256+RSAで署名する
            var signerFactory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, keyPair.Private);
            var x509 = x509gen.Generate(signerFactory);
            return new X509Certificate2(DotNetUtilities.ToX509Certificate(x509));
        }
    }
}
