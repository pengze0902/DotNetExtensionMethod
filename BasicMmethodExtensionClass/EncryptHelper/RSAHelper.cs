using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// 非对称RSA加密类
    /// 需要BigInteger类来辅助
    /// </summary>
    public static class RsaHelper
    {
        /// <summary>
        /// RSA的容器 可以解密的源字符串长度为 DWKEYSIZE/8-11 
        /// </summary>
        public const int Dwkeysize = 1024;

        /// <summary>
        /// RSA加密的密匙结构  公钥和私匙
        /// </summary>
        public struct RsaKey
        {
            public string PublicKey { get; set; }

            public string PrivateKey { get; set; }
        }

        /// <summary>
        /// 得到RSA的解谜的密匙对
        /// </summary>
        /// <returns></returns>
        public static RsaKey GetRasKey()
        {
            RSACryptoServiceProvider.UseMachineKeyStore = true;
            //声明一个指定大小的RSA容器
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(Dwkeysize);
            //取得RSA容易里的各种参数
            RSAParameters p = rsaProvider.ExportParameters(true);

            return new RsaKey
            {
                PublicKey = ComponentKey(p.Exponent, p.Modulus),
                PrivateKey = ComponentKey(p.D, p.Modulus)
            };
        }

        /// <summary>
        /// 检查明文的有效性 DWKEYSIZE/8-11 长度之内为有效 中英文都算一个字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckSourceValidate(string source)
        {
            return (Dwkeysize / 8 - 11) >= source.Length;
        }

        /// <summary>
        /// 组合成密匙字符串
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        private static string ComponentKey(byte[] b1, byte[] b2)
        {
            var list = new List<byte>
            {
                (byte) b1.Length
            };
            list.AddRange(b1);
            list.AddRange(b2);
            var b = list.ToArray<byte>();
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 解析密匙
        /// </summary>
        /// <param name="key">密匙</param>
        /// <param name="b1">RSA的相应参数1</param>
        /// <param name="b2">RSA的相应参数2</param>
        private static void ResolveKey(string key, out byte[] b1, out byte[] b2)
        {
            //从base64字符串 解析成原来的字节数组
            byte[] b = Convert.FromBase64String(key);
            //初始化参数的数组长度
            b1 = new byte[b[0]];
            b2 = new byte[b.Length - b[0] - 1];
            //将相应位置是值放进相应的数组
            for (int n = 1, i = 0, j = 0; n < b.Length; n++)
            {
                if (n <= b[0])
                {
                    b1[i++] = b[n];
                }
                else
                {
                    b2[j++] = b[n];
                }
            }
        }


        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="source">源字符串 明文</param>
        /// <param name="key">密匙</param>
        /// <returns>加密遇到错误将会返回原字符串</returns>
        public static string EncryptString(string source, string key)
        {
            string encryptString;
            try
            {
                if (!CheckSourceValidate(source))
                {
                    throw new Exception("明文太长");
                }
                //解析这个密钥
                byte[] d;
                byte[] n;
                ResolveKey(key, out d, out n);
                var biN = new BigInteger(n);
                var biD = new BigInteger(d);
                encryptString = EncryptString(source, biD, biN);
            }
            catch
            {
                encryptString = source;
            }
            return encryptString;
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="encryptString">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>遇到解密失败将会返回原字符串</returns>
        public static string DecryptString(string encryptString, string key)
        {
            string source;
            try
            {
                //解析这个密钥
                byte[] e;
                byte[] n;
                ResolveKey(key, out e, out n);
                var biE = new BigInteger(e);
                var biN = new BigInteger(n);
                source = DecryptString(encryptString, biE, biN);
            }
            catch
            {
                source = encryptString;
            }
            return source;
        }

        /// <summary>
        /// 用指定的密匙加密 
        /// </summary>
        /// <param name="source">明文</param>
        /// <param name="d">可以是RSACryptoServiceProvider生成的D</param>
        /// <param name="n">可以是RSACryptoServiceProvider生成的Modulus</param>
        /// <returns>返回密文</returns>
        private static string EncryptString(string source, BigInteger d, BigInteger n)
        {
            var len = source.Length;
            int len1;
            if ((len % 128) == 0)
                len1 = len / 128;
            else
                len1 = len / 128 + 1;
            var result = new StringBuilder();
            for (var i = 0; i < len1; i++)
            {
                var blockLen = len >= 128 ? 128 : len;
                var block = source.Substring(i * 128, blockLen);
                byte[] oText = Encoding.UTF8.GetBytes(block);
                var biText = new BigInteger(oText);
                var biEnText = biText.modPow(d, n);
                var temp = biEnText.ToHexString();
                result.Append(temp).Append("@");
                len -= blockLen;
            }
            return result.ToString().TrimEnd('@');
        }

        /// <summary>
        /// 用指定的密匙加密 
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="e">可以是RSACryptoServiceProvider生成的Exponent</param>
        /// <param name="n">可以是RSACryptoServiceProvider生成的Modulus</param>
        /// <returns>返回明文</returns>
        private static string DecryptString(string encryptString, BigInteger e, BigInteger n)
        {
            var result = new StringBuilder();
            var strarr1 = encryptString.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var block in strarr1)
            {
                var biText = new BigInteger(block, 16);
                var biEnText = biText.modPow(e, n);
                var temp = Encoding.UTF8.GetString(biEnText.getBytes());
                result.Append(temp);
            }
            return result.ToString();
        }
    }
}
