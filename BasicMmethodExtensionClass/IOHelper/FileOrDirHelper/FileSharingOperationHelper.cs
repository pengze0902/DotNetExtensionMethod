using System;
using System.Diagnostics;
using System.IO;

namespace BasicMmethodExtensionClass.IOHelper.FileOrDirHelper
{
    /// <summary>
    /// 共享文档操作
    /// </summary>
    public class FileSharingOperationHelper
    {
        public static bool ConnectState(string path)
        {
            return ConnectState(path, "", "");
        }

        /// <summary>
        /// 连接远程共享文件夹
        /// </summary>
        /// <param name="path">远程共享文件夹的路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static bool ConnectState(string path, string userName, string passWord)
        {
            var proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                var dosLine = "net use " + path + " " + passWord + " /user:" + userName;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                var errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (!string.IsNullOrEmpty(errormsg))
                    throw new Exception(errormsg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return true;
        }

        /// <summary>
        /// 向远程文件夹保存本地内容，或者从远程文件夹下载文件到本地
        /// </summary>
        /// <param name="src">要保存的文件的路径，如果保存文件到共享文件夹，这个路径就是本地文件路径如：@"D:\1.avi"</param>
        /// <param name="dst">保存文件的路径，不含名称及扩展名</param>
        /// <param name="fileName">保存文件的名称以及扩展名</param>
        public static void Transport(string src, string dst, string fileName)
        {
            var inFileStream = new FileStream(src, FileMode.Open);
            if (!Directory.Exists(dst))
            {
                Directory.CreateDirectory(dst);
            }
            dst = dst + fileName;
            var outFileStream = new FileStream(dst, FileMode.OpenOrCreate);
            var buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
            {
                outFileStream.Write(buf, 0, byteCount);
            }
            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
        }
    }
}

/*
 * 操作示例：
          bool status = false;
            //连接共享文件夹
            status = connectState(@"\\10.200.8.73\share", "administrator", "11111111");
            if (status)
            {
                //共享文件夹的目录
                DirectoryInfo theFolder = new DirectoryInfo(@"\\10.200.8.73\share");
                //相对共享文件夹的路径
                string fielpath=@"\123\456\";
                //获取保存文件的路径
                string filename = theFolder.ToString() +fielpath ;
                //执行方法
                Transport(@"D:\1.jpg", filename, "1.jpg");

            }
            else
            {
                //ListBox1.Items.Add("未能连接！");
            }
            Console.ReadKey();
 */