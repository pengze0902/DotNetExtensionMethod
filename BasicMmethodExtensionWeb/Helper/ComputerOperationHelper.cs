using System;
using System.Management;

namespace BasicMmethodExtensionWeb.Helper
{
    public class ComputerOperationHelper
    {
        /// <summary>
        /// cpu序列号
        /// </summary>
        public static string CpuId;

        /// <summary>
        /// mac序列号
        /// </summary>
        public static string MacAddress;

        /// <summary>
        /// 硬盘id
        /// </summary>
        public static string DiskId;

        /// <summary>
        /// ip地址
        /// </summary>
        public static string IpAddress;

        /// <summary>
        /// 登录用户名
        /// </summary>
        public static string LoginUserName;

        /// <summary>
        /// 计算机名
        /// </summary>
        public static string ComputerName;

        /// <summary>
        /// 系统类型
        /// </summary>
        public static string SystemType;

        /// <summary>
        /// 内存量 单位：M
        /// </summary>
        public static string TotalPhysicalMemory;

        /// <summary>
        /// 类型构造函数
        /// </summary>
        static ComputerOperationHelper()
        {
            CpuId = GetCpuId();
            MacAddress = GetMacAddress();
            DiskId = GetDiskId();
            IpAddress = GetIpAddress();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();
        }


        /// <summary>
        /// 获取CPU序列号代码 
        /// </summary>
        /// <returns></returns>
        public static string GetCpuId()
        {
            var cpuInfo = string.Empty;
            var mc = new ManagementClass("Win32_Processor");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuInfo;

        }

        /// <summary>
        /// 获取网卡硬件地址 
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            var mac = string.Empty;
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if ((bool)mo["IPEnabled"] != true)
                {
                    continue;
                }
                mac = mo["MacAddress"].ToString();
                break;
            }
            return mac;
        }

        /// <summary>
        /// 获取硬盘ID 
        /// </summary>
        /// <returns></returns>
        public static string GetDiskId()
        {

            var hDid = string.Empty;
            var mc = new ManagementClass("Win32_DiskDrive");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                hDid = (string)mo.Properties["Model"].Value;
            }
            return hDid;
        }


        /// <summary>
        /// 获取IP地址 
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            var st = string.Empty;
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if ((bool)mo["IPEnabled"] != true) continue;
                var ar = (Array)(mo.Properties["IpAddress"].Value);
                st = ar.GetValue(0).ToString();
                break;
            }
            return st;
        }

        /// <summary>
        /// 操作系统的登录用户名 
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            var un = Environment.UserName;
            return un;
        }



        /// <summary>
        /// 获取计算机名
        /// </summary>
        /// <returns></returns>
        static string GetComputerName()
        {
            return Environment.MachineName;
        }



        /// <summary>
        /// PC类型 
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType()
        {
            var st = string.Empty;
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                st = mo["SystemType"].ToString();
            }
            return st;
        }


        /// <summary>
        /// 物理内存 
        /// </summary>
        /// <returns></returns>        
        public static string GetTotalPhysicalMemory()
        {
            var st = string.Empty;
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                st = mo["TotalPhysicalMemory"].ToString();
            }
            return st;
        }
    }
}
