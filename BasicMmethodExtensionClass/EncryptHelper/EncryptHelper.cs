using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// 加密类，加密方式为MD5
    /// </summary>
    public class EncryptHelper
    {
        // DES密钥，长度至少为8且不能为中文
        private const string DesrgbKey = "ox$4^&3H~K5%";
        private const string DesRgbIv = "&*56YMd#GHpJ";

        /// <summary>
        /// MD5加密字符串（加密不可逆）
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>加密后的字符串</returns>
        //public static string Md5Encrypt(string targetString)
        //{
        //    var sb = new StringBuilder();
        //    var md5 = Md5.Create();
        //    var bts = md5.ComputeHash(Encoding.Unicode.GetBytes(targetString));

        //    foreach (var bt in bts)
        //    {
        //        sb.Append(bt);
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DesEncrypt(string targetString)
        {
            try
            {
                var rgbKey = Encoding.UTF8.GetBytes(DesrgbKey.Substring(0, 8));
                var rgbIv = Encoding.UTF8.GetBytes(DesRgbIv.Substring(0, 8));
                var inputByte = Encoding.UTF8.GetBytes(targetString);
                var serviceProvider = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, serviceProvider.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return targetString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DesDecrypt(string targetString)
        {
            try
            {
                var rgbKey = Encoding.UTF8.GetBytes(DesrgbKey.Substring(0, 8));
                var rgbIv = Encoding.UTF8.GetBytes(DesRgbIv.Substring(0, 8));
                var inputByte = Convert.FromBase64String(targetString);
                var serviceProvider = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, serviceProvider.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return targetString;
            }
        }
    }
}