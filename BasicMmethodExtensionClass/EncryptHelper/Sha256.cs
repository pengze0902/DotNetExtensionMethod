using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// Sha256 utils类
    /// </summary>
    public static class Sha256
    {
        /// <summary>
        /// 将字符串转换为sha256散列
        /// </summary>
        /// <param name="data">字符串进行转换</param>
        /// <returns>sha256散列或null</returns>
        public static string ToSha256(this string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    return null;

                var hashValue = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(data));
                var hex = hashValue.Aggregate("", (current, x) => current + String.Format("{0:x2}", x));

                if (string.IsNullOrEmpty(hex))
                    throw new Exception("Erro creating SHA256 hash");

                return hex;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
