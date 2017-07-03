using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyCodeReceiver
{
    public class KeyReceiver
    {
        private string ip;
        private Keyboarder keyboarder;
        private byte[] sessionKey;
        private UdpClient server;
        private string pw;
        private Form1 mainThreadForm;
        private bool isRunning;
        private long preReceiveTime;

        public KeyReceiver(Form1 mainThreadForm)
        {
            ip = GetIPAddress();
            keyboarder = new Keyboarder();
            this.mainThreadForm = mainThreadForm;
        }

        public void Run(int port, string pw)
        {
            this.pw = pw;
            isRunning = true;
            Task.Run(() =>
            {
                try
                {
                    var localEP = new IPEndPoint(IPAddress.Parse(ip), port);
                    server = new UdpClient(localEP);
                    mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, "サーバ起動");

                    while (true)
                    {
                        IPEndPoint remoteEP = null;
                        var receiveBytes = server.Receive(ref remoteEP);

                        if (sessionKey == null)
                        {
                            ExchangeSessionKey(remoteEP.Address.ToString(), port);
                        }

                        var clientMsg = Encoding.UTF8.GetString(receiveBytes);
                        var splitted = clientMsg.Split(new string[] { "?" }, StringSplitOptions.None);
                        var iv = Convert.FromBase64String(splitted[0]);
                        var encrypted = Convert.FromBase64String(splitted[1]);

                        if (sessionKey == null)
                        {
                            mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, "パスワードが一致しません");
                            Stop();
                            return;
                        }
                        string plain;
                        try
                        {
                            plain = Encoding.UTF8.GetString(Decrypt(iv, encrypted));
                        }
                        catch (CryptographicException)
                        {
                            //クライアント側でセッションキーが変更されている
                            ExchangeSessionKey(remoteEP.Address.ToString(), port);
                            plain = Encoding.UTF8.GetString(Decrypt(iv, encrypted));
                        }

                        var timeAndKeyCodes = plain.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        var time = long.Parse(timeAndKeyCodes[0]);
                        if (preReceiveTime != long.MaxValue && time > preReceiveTime)
                        {
                            preReceiveTime = time;
                        }
                        else
                        {
                            Console.WriteLine("replay attack? from " + remoteEP.Address);
                            Stop();
                        }

                        var keyCodes = timeAndKeyCodes[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        keyboarder.InputKeys(keyCodes);
                        var log = keyCodes[0];
                        foreach (int i in Enumerable.Range(1, keyCodes.Length - 1))
                        {
                            log += "+" + keyCodes[i];
                        }
                        mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, log);
                    }
                }
                catch (Exception e)
                {
                    if (isRunning)
                    {
                        mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, "エラー発生");
                        Console.WriteLine(e.ToString());
                        Stop();
                    }
                }
            });
        }

        public void Stop()
        {
            if (isRunning)
            {
                mainThreadForm.Invoke(mainThreadForm.updateRunButtonTextDelegate, "起動");
                isRunning = false;
                server.Close();
                sessionKey = null;
                mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, "サーバ停止");
            }
        }

        public static string GetIPAddress()
        {
            var ipaddress = "";
            IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }
            return ipaddress;
        }

        private void ExchangeSessionKey(string remoteIp, int port)
        {
            Console.WriteLine("start exchanging key");

            //ECDH鍵共有のための鍵ペアを生成
            var ecKeyGen = new ECKeyPairGenerator();
            var keyGenParam = new KeyGenerationParameters(new SecureRandom(), 256);
            ecKeyGen.Init(keyGenParam);
            var keyPair = ecKeyGen.GenerateKeyPair();

            using (var tcpClient = new TcpClient(remoteIp, port))
            using (var netStream = tcpClient.GetStream())
            using (var reader = new StreamReader(netStream))
            using (var writer = new StreamWriter(netStream))
            {
                //公開情報を送信
                writer.WriteLine(Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public).GetEncoded(), Base64FormattingOptions.None));
                writer.Flush();

                //公開情報を受信
                var kotlinPubKey = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(reader.ReadLine()));

                //共通鍵を生成
                var keyAgree = AgreementUtilities.GetBasicAgreement("ECDH");
                keyAgree.Init(keyPair.Private);
                var sharedSecret = keyAgree.CalculateAgreement(kotlinPubKey);
                var dhSecKey = sharedSecret.ToByteArrayUnsigned();

                var gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
                gen.Init(Encoding.UTF8.GetBytes(pw), dhSecKey, 4096);
                byte[] secretKey = ((KeyParameter)gen.GenerateDerivedParameters(256)).GetKey();

                var splitted = reader.ReadLine().Split(new string[] { "?" }, StringSplitOptions.None);
                var iv = Convert.FromBase64String(splitted[0]);
                var encrypted = Convert.FromBase64String(splitted[1]);

                try
                {
                    sessionKey = Decrypt(iv, encrypted, secretKey);
                    writer.WriteLine(Encrypt("ok"));
                    Console.WriteLine("succeeded in exchanging key");
                }
                catch (CryptographicException)
                {
                    writer.WriteLine("e1");
                    mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, "パスワードが一致していません");
                    Stop();
                }
                writer.Flush();

                splitted = reader.ReadLine().Split(new string[] { "?" }, StringSplitOptions.None);
                iv = Convert.FromBase64String(splitted[0]);
                encrypted = Convert.FromBase64String(splitted[1]);
                var clientName = Encoding.UTF8.GetString(Decrypt(iv, encrypted));
                mainThreadForm.Invoke(mainThreadForm.writeLogDelegate, clientName + "が接続しました");
            }
        }

        private string Encrypt(string text)
        {
            // AES暗号化サービスプロバイダ
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = sessionKey;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // 文字列をバイト型配列に変換
            byte[] src = Encoding.UTF8.GetBytes(text);

            // 暗号化する
            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                // バイト型配列からBase64形式の文字列に変換
                return Convert.ToBase64String(aes.IV, Base64FormattingOptions.None) + "?" + Convert.ToBase64String(dest, Base64FormattingOptions.None);
            }
        }

        private byte[] Decrypt(byte[] iv, byte[] encrypted)
        {
            return Decrypt(iv, encrypted, sessionKey);
        }

        private byte[] Decrypt(byte[] iv, byte[] encrypted, byte[] key)
        {
            var csp = new AesCryptoServiceProvider();
            csp.BlockSize = 128;
            csp.KeySize = 256;
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            csp.IV = iv;
            csp.Key = key;

            using (ICryptoTransform decrypt = csp.CreateDecryptor())
            {
                return decrypt.TransformFinalBlock(encrypted, 0, encrypted.Length);
            }
        }
    }
}
