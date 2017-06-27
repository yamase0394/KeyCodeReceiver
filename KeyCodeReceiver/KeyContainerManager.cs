using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyCodeReceiver
{
    class KeyContainerManager
    {
        /// <summary>
        /// 公開鍵と秘密鍵を作成し、キーコンテナに格納する
        /// </summary>
        /// <param name="containerName">キーコンテナ名</param>
        /// <returns>作成された公開鍵(XML形式)</returns>
        public static string CreateKeys(string containerName)
        {
            //CspParametersオブジェクトの作成
            System.Security.Cryptography.CspParameters cp =
                new System.Security.Cryptography.CspParameters();
            //キーコンテナ名を指定する
            cp.KeyContainerName = containerName;
            //CspParametersを指定してRSACryptoServiceProviderオブジェクトを作成
            System.Security.Cryptography.RSACryptoServiceProvider rsa =
                new System.Security.Cryptography.RSACryptoServiceProvider(cp);

            //公開鍵をXML形式で取得して返す
            return rsa.ToXmlString(false);
        }

        /// <summary>
        /// 公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵(XML形式)</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string str, string publicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa =
                new System.Security.Cryptography.RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] encryptedData = rsa.Encrypt(data, false);
            return System.Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// キーコンテナに格納された秘密鍵を使って、文字列を復号化する
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <param name="containerName">キーコンテナ名</param>
        /// <returns>復号化された文字列</returns>
        public static string Decrypt(string str, string containerName)
        {
            //CspParametersオブジェクトの作成
            System.Security.Cryptography.CspParameters cp =
                new System.Security.Cryptography.CspParameters();
            //キーコンテナ名を指定する
            cp.KeyContainerName = containerName;
            //CspParametersを指定してRSACryptoServiceProviderオブジェクトを作成
            System.Security.Cryptography.RSACryptoServiceProvider rsa =
                new System.Security.Cryptography.RSACryptoServiceProvider(cp);

            //復号化する
            byte[] data = System.Convert.FromBase64String(str);
            byte[] decryptedData = rsa.Decrypt(data, false);
            return System.Text.Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// 指定されたキーコンテナを削除する
        /// </summary>
        /// <param name="containerName">キーコンテナ名</param>
        public static void DeleteKeys(string containerName)
        {
            //CspParametersオブジェクトの作成
            System.Security.Cryptography.CspParameters cp =
                new System.Security.Cryptography.CspParameters();
            //キーコンテナ名を指定する
            cp.KeyContainerName = containerName;
            //CspParametersを指定してRSACryptoServiceProviderオブジェクトを作成
            System.Security.Cryptography.RSACryptoServiceProvider rsa =
                new System.Security.Cryptography.RSACryptoServiceProvider(cp);

            //キーコンテナを削除
            rsa.PersistKeyInCsp = false;
            rsa.Clear();
        }
    }
}
