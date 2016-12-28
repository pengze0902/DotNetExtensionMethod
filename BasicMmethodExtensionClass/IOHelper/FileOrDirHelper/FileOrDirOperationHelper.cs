using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BasicMmethodExtensionClass.IOHelper.FileOrDirHelper
{
    public class FileOrDirOperationHelper
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string fileName)
        {
            if (fileName == null || fileName.Trim() == "")
            {
                return false;
            }
            return System.IO.File.Exists(fileName);
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirName">文件夹名</param>
        /// <returns></returns>
        public static bool CreateDir(string dirName)
        {
            try
            {
                if (dirName == null)
                    throw new Exception("dirName");
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                return true;
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }
        }


        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>创建失败返回false</returns>
        public static bool CreateFile(string fileName)
        {
            try
            {
                if (System.IO.File.Exists(fileName)) return false;
                var fs = System.IO.File.Create(fileName);
                fs.Close();
                fs.Dispose();
            }
            catch (IOException ioe)
            {
                throw new IOException(ioe.Message);
            }

            return true;
        }


        /// <summary>
        /// 读文件内容,转化为字符类型
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static string Read(string fileName)
        {
            if (!Exists(fileName))
            {
                return null;
            }
            //将文件信息读入流中
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                return new StreamReader(fs).ReadToEnd();
            }
        }


        /// <summary>
        /// 文件转化为Char[]数组
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static char[] FileRead(string fileName)
        {
            if (!Exists(fileName))
            {
                return null;
            }
            var byData = new byte[1024];
            var charData = new char[1024];
            try
            {
                var fileStream = new FileStream(fileName, FileMode.Open);
                fileStream.Seek(135, SeekOrigin.Begin);
                fileStream.Read(byData, 0, 1024);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var decoder = Encoding.UTF8.GetDecoder();
            decoder.GetChars(byData, 0, byData.Length, charData, 0);
            return charData;
        }



        /// <summary>
        /// 文件转化为byte[]
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static byte[] ReadFile(string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var r = new BinaryReader(pFileStream);
                //将文件指针设置到文件开
                r.BaseStream.Seek(0, SeekOrigin.Begin);
                var pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                if (pFileStream != null) pFileStream.Close();
            }
        }


        /// <summary>
        /// 将byte写入文件
        /// </summary>
        /// <param name="pReadByte">字节流</param>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static bool WriteFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (pFileStream != null) pFileStream.Close();
            }
            return true;

        }

        public static string ReadLine(string fileName)
        {
            if (!Exists(fileName))
            {
                return null;
            }
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                return new StreamReader(fs).ReadLine();
            }
        }


        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">文件内容</param>
        /// <returns></returns>
        public static bool Write(string fileName, string content)
        {
            if (Exists(fileName) || content == null)
            {
                return false;
            }
            try
            {
                //将文件信息读入流中
                //初始化System.IO.FileStream类的新实例与指定路径和创建模式
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    //锁住流
                    lock (fs)
                    {
                        if (!fs.CanWrite)
                        {
                            throw new System.Security.SecurityException("文件fileName=" + fileName + "是只读文件不能写入!");
                        }

                        var buffer = Encoding.Default.GetBytes(content);
                        fs.Write(buffer, 0, buffer.Length);
                        return true;
                    }
                }
            }
            catch (IOException ioe)
            {
                throw new Exception(ioe.Message);
            }

        }


        /// <summary>
        /// 写入一行
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static bool WriteLine(string fileName, string content)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(fileName);
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(content);
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate | FileMode.Append))
            {
                //锁住流
                lock (fs)
                {
                    if (!fs.CanWrite)
                    {
                        throw new System.Security.SecurityException("文件fileName=" + fileName + "是只读文件不能写入!");
                    }

                    var sw = new StreamWriter(fs);
                    sw.WriteLine(content);
                    sw.Dispose();
                    sw.Close();
                    return true;
                }
            }
        }


        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="fromDir">被复制的目录</param>
        /// <param name="toDir">复制到的目录</param>
        /// <returns></returns>
        public static bool CopyDir(DirectoryInfo fromDir, string toDir)
        {
            return CopyDir(fromDir, toDir, fromDir.FullName);
        }


        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="fromDir">被复制的目录</param>
        /// <param name="toDir">复制到的目录</param>
        /// <returns></returns>
        public static bool CopyDir(string fromDir, string toDir)
        {
            if (fromDir == null || toDir == null)
            {
                throw new NullReferenceException("参数为空");
            }

            if (fromDir == toDir)
            {
                throw new Exception("两个目录都是" + fromDir);
            }

            if (!Directory.Exists(fromDir))
            {
                throw new IOException("目录fromDir=" + fromDir + "不存在");
            }

            var dir = new DirectoryInfo(fromDir);
            return CopyDir(dir, toDir, dir.FullName);
        }


        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="fromDir">被复制的目录</param>
        /// <param name="toDir">复制到的目录</param>
        /// <param name="rootDir">被复制的根目录</param>
        /// <returns></returns>
        private static bool CopyDir(DirectoryInfo fromDir, string toDir, string rootDir)
        {
            foreach (var f in fromDir.GetFiles())
            {
                var filePath = toDir + f.FullName.Substring(rootDir.Length);
                var newDir = filePath.Substring(0, filePath.LastIndexOf("\\", StringComparison.Ordinal));
                CreateDir(newDir);
                System.IO.File.Copy(f.FullName, filePath, true);
            }

            foreach (var dir in fromDir.GetDirectories())
            {
                CopyDir(dir, toDir, rootDir);
            }

            return true;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件的完整路径</param>
        /// <returns></returns>
        public static bool DeleteFile(string fileName)
        {
            try
            {
                if (!Exists(fileName)) return false;
                System.IO.File.Delete(fileName);
            }
            catch (IOException ioe)
            {
                throw new ArgumentNullException(ioe.Message);
            }

            return true;
        }


        public static void DeleteDir(DirectoryInfo dir)
        {
            if (dir == null)
            {
                throw new NullReferenceException("目录不存在");
            }

            foreach (var d in dir.GetDirectories())
            {
                DeleteDir(d);
            }

            foreach (var f in dir.GetFiles())
            {
                DeleteFile(f.FullName);
            }

            dir.Delete();

        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">指定目录</param>
        /// <param name="onlyDir">是否只删除目录</param>
        /// <returns></returns>
        public static bool DeleteDir(string dir, bool onlyDir)
        {
            if (dir == null || dir.Trim() == "")
            {
                throw new NullReferenceException("目录dir=" + dir + "不存在");
            }

            if (!Directory.Exists(dir))
            {
                return false;
            }

            var dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles().Length == 0 && dirInfo.GetDirectories().Length == 0)
            {
                Directory.Delete(dir);
                return true;
            }


            if (!onlyDir)
            {
                return false;
            }
            DeleteDir(dirInfo);
            return true;
        }


        /// <summary>
        /// 在指定的目录中查找文件
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool FindFile(string dir, string fileName)
        {
            if (dir == null || dir.Trim() == "" || fileName == null || fileName.Trim() == "" || !Directory.Exists(dir))
            {
                return false;
            }

            //传入文件路径，获取当前文件对象
            var dirInfo = new DirectoryInfo(dir);
            return FindFile(dirInfo, fileName);

        }


        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool FindFile(DirectoryInfo dir, string fileName)
        {
            foreach (var d in dir.GetDirectories())
            {
                if (System.IO.File.Exists(d.FullName + "\\" + fileName))
                {
                    return true;
                }
                FindFile(d, fileName);
            }

            return false;
        }


        /// <summary>
        /// 获取指定文件夹中的所有文件夹名称
        /// </summary>
        /// <param name="folderPath">路径</param>
        /// <returns></returns>
        public static List<string> FolderName(string folderPath)
        {
            var listFolderName = new List<string>();
            try
            {
                var info = new DirectoryInfo(folderPath);

                listFolderName.AddRange(info.GetDirectories().Select(nextFolder => nextFolder.Name));
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }

            return listFolderName;

        }


        /// <summary>
        /// 获取指定文件夹中的文件名称
        /// </summary>
        /// <param name="folderPath">路径</param>
        /// <returns></returns>
        public static List<string> FileName(string folderPath)
        {
            var listFileName = new List<string>();
            try
            {
                var info = new DirectoryInfo(folderPath);

                listFileName.AddRange(info.GetFiles().Select(nextFile => nextFile.Name));
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }

            return listFileName;
        }
    }
}
