using System;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// 用于文本和Base64编码文本的互相转换 和 Byte[]和Base64编码文本的互相转换
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// 将普通文本转换成Base64编码的文本
        /// </summary>
        /// <param name="value">普通文本</param>
        /// <returns></returns>
        public static string StringToBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }
            try
            {
                var binBuffer = (new UnicodeEncoding()).GetBytes(value);
                var base64ArraySize = (int)Math.Ceiling(binBuffer.Length / 3d) * 4;
                var charBuffer = new char[base64ArraySize];
                Convert.ToBase64CharArray(binBuffer, 0, binBuffer.Length, charBuffer, 0);
                var s = new string(charBuffer);
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 将Base64编码的文本转换成普通文本
        /// </summary>
        /// <param name="base64">Base64编码的文本</param>
        /// <returns></returns>
        public static string Base64StringToString(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                throw new ArgumentNullException(base64);
            }
            try
            {
                var charBuffer = base64.ToCharArray();
                var bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                return (new UnicodeEncoding()).GetString(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 将Byte[]转换成Base64编码文本
        /// </summary>
        /// <param name="binBuffer">Byte[]</param>
        /// <returns></returns>
        public string ToBase64(byte[] binBuffer)
        {
            if (binBuffer == null)
            {
                throw new ArgumentNullException("binBuffer");
            }
            try
            {
                var base64ArraySize = (int)Math.Ceiling(binBuffer.Length / 3d) * 4;
                var charBuffer = new char[base64ArraySize];
                Convert.ToBase64CharArray(binBuffer, 0, binBuffer.Length, charBuffer, 0);
                var s = new string(charBuffer);
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 将Base64编码文本转换成Byte[]
        /// </summary>
        /// <param name="base64">Base64编码文本</param>
        /// <returns></returns>
        public byte[] Base64ToBytes(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                throw new ArgumentNullException(base64);
            }
            try
            {
                var charBuffer = base64.ToCharArray();
                var bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
