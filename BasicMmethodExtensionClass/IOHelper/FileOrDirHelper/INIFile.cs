using System.Runtime.InteropServices;
using System.Text;

namespace BasicMmethodExtensionClass.IOHelper.FileOrDirHelper
{
    /// <summary>
    /// INI文件读写类。
    /// </summary>
	public class IniFile
	{
		public string path;

		public IniFile(string iniPath)
		{
			path = iniPath;
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal,int size,string filePath);

	
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string defVal, System.Byte[] retVal, int size, string filePath);


		/// <summary>
		/// 写INI文件
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void IniWriteValue(string section,string key,string value)
		{
			WritePrivateProfileString(section,key,value,this.path);
		}

		/// <summary>
		/// 读取INI文件
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string IniReadValue(string section,string key)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(section,key,"",temp, 255, this.path);
			return temp.ToString();
		}
		public byte[] IniReadValues(string section, string key)
		{
			byte[] temp = new byte[255];
			int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
			return temp;

		}


		/// <summary>
		/// 删除ini文件下所有段落
		/// </summary>
		public void ClearAllSection()
		{
			IniWriteValue(null,null,null);
		}
		/// <summary>
		/// 删除ini文件下personal段落下的所有键
		/// </summary>
		/// <param name="section"></param>
		public void ClearSection(string section)
		{
			IniWriteValue(section,null,null);
		}

	}


}
