using System;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    public class Sha1UtilHelper
    {
        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSha1(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(str);
            }
            try
            {
                //建立SHA1对象
                SHA1 sha = new SHA1CryptoServiceProvider();
                //将mystr转换成byte[] 
                var enc = new ASCIIEncoding();
                var dataToHash = enc.GetBytes(str);
                //Hash运算
                var dataHashed = sha.ComputeHash(dataToHash);
                //将运算结果转换成string
                var hash = BitConverter.ToString(dataHashed).Replace("-", "");
                return hash;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}