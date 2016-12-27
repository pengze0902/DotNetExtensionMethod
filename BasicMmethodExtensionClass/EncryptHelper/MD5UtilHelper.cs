using System;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
	/// <summary>
    /// MD5UtilHelper 的摘要说明。
	/// </summary>
	public class Md5UtilHelper
	{
        public Md5UtilHelper()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		/// <summary>
        /// 获取大写的MD5签名结果
		/// </summary>
		/// <param name="encypStr"></param>
		/// <param name="charset"></param>
		/// <returns></returns>
		public static string GetMd5(string encypStr, string charset)
		{
		    var m5 = new MD5CryptoServiceProvider();

			//创建md5对象
			byte[] inputBye;

		    //使用GB2312编码方式把字符串转化为字节数组．
			try
			{
				inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
			}
			catch (Exception ex)
			{
				inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
			}
			var outputBye = m5.ComputeHash(inputBye);

			var retStr = BitConverter.ToString(outputBye);
			retStr = retStr.Replace("-", "").ToUpper();
			return retStr;
		}
	}
}
