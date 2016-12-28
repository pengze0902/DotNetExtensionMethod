using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace BasicMmethodExtensionClass.IOHelper.FileOrDirHelper
{
    /// <summary>
    /// 粉碎文件操作
    /// </summary>
    public class KillFileHelper
    {
        /// <summary>
        /// 强力粉碎文件，文件如果被打开，很难粉碎
        /// </summary>
        /// <param name="filename">文件全路径</param>
        /// <param name="deleteCount">删除次数</param>
        /// <param name="randomData">随机数据填充文件，默认true</param>
        /// <param name="blanks">空白填充文件，默认false</param>
        /// <returns>true：粉碎成功，false：粉碎失败</returns>
        public static bool KillFile(string filename, int deleteCount, bool randomData = true, bool blanks = false)
        {
            const int bufferLength = 1024000;
            var ret = true;
            try
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var f = new FileInfo(filename);
                    var count = f.Length;
                    long offset = 0;
                    var rowDataBuffer = new byte[bufferLength];
                    while (count >= 0)
                    {
                        var iNumOfDataRead = stream.Read(rowDataBuffer, 0, bufferLength);
                        if (iNumOfDataRead == 0)
                        {
                            break;
                        }
                        if (randomData)
                        {
                            var randombyte = new Random();
                            randombyte.NextBytes(rowDataBuffer);
                        }
                        else if (blanks)
                        {
                            for (var i = 0; i < iNumOfDataRead; i++)
                                rowDataBuffer[i] = 0;
                        }
                        else
                        {
                            for (var i = 0; i < iNumOfDataRead; i++)
                                rowDataBuffer[i] = Convert.ToByte(Convert.ToChar(deleteCount));
                        }
                        // 写新内容到文件。
                        for (var i = 0; i < deleteCount; i++)
                        {
                            stream.Seek(offset, SeekOrigin.Begin);
                            stream.Write(rowDataBuffer, 0, iNumOfDataRead);
                        }
                        offset += iNumOfDataRead;
                        count -= iNumOfDataRead;
                    }
                }
                //每一个文件名字符代替随机数从0到9。
                var newName = "";
                do
                {
                    var random = new Random();
                    var cleanName = Path.GetFileName(filename);
                    var dirName = Path.GetDirectoryName(filename);
                    var iMoreRandomLetters = random.Next(9);
                    // 为了更安全，不要只使用原文件名的大小，添加一些随机字母。
                    for (var i = 0; i < cleanName.Length + iMoreRandomLetters; i++)
                    {
                        newName += random.Next(9).ToString();
                    }
                    newName = dirName + "\\" + newName;
                } while (System.IO.File.Exists(newName));
                // 重命名文件的新的随机的名字。
                System.IO.File.Move(filename, newName);
                System.IO.File.Delete(newName);
            }
            catch
            {
                //可能其他原因删除失败了，使用我们自己的方法强制删除
                var matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
                try
                {
                    var fileName = filename;//要检查被那个进程占用的文件
                    var tool = new Process { StartInfo = { FileName = "handle.exe", Arguments = fileName + " /accepteula", UseShellExecute = false, RedirectStandardOutput = true } };
                    tool.Start();
                    tool.WaitForExit();
                    var outputTool = tool.StandardOutput.ReadToEnd();
                    foreach (Match match in System.Text.RegularExpressions.Regex.Matches(outputTool, matchPattern))
                    {
                        //结束掉所有正在使用这个文件的程序
                        Process.GetProcessById(int.Parse(match.Value)).Kill();
                    }
                    System.IO.File.Delete(fileName);
                }
                catch
                {
                    ret = false;
                }
            }
            return ret;
        }
    }
}