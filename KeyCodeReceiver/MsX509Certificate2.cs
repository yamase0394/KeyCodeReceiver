using System.Security.Cryptography.X509Certificates;

namespace KeyCodeReceiver
{
    internal class MsX509Certificate2
    {
        private X509Certificate x509Certificate;

        public MsX509Certificate2(X509Certificate x509Certificate)
        {
            this.x509Certificate = x509Certificate;
        }
    }
}