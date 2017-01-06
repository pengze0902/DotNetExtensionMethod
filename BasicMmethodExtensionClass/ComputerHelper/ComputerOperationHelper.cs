using System;
using System.Management;

namespace Helper.ComputerHelper
{
    public class ComputerOperationHelper
    {
        //1.cpu序列号
        public static string CpuId;

        //2.mac序列号
        public static string MacAddress;

        //3.硬盘id
        public static string DiskId;

        //4.ip地址
        public static string IpAddress;

        //5.登录用户名
        public static string LoginUserName;

        //6.计算机名
        public static string ComputerName;

        //7.系统类型
        public static string SystemType;

        //8.内存量 单位：M
        public static string TotalPhysicalMemory;

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


        //1.获取CPU序列号代码 
        public static string GetCpuId()
        {
            try
            {
                //cpu序列号 
                var cpuInfo = "";
                var mc = new ManagementClass("Win32_Processor");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                return cpuInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //2.获取网卡硬件地址 
        public static string GetMacAddress()
        {
            try
            {
                var mac = "";
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    if ((bool)mo["IPEnabled"] != true) continue;
                    mac = mo["MacAddress"].ToString();
                    break;
                }
                return mac;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //3.获取硬盘ID 
        public static string GetDiskId()
        {
            try
            {
                var hDid = "";
                var mc = new ManagementClass("Win32_DiskDrive");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    hDid = (string)mo.Properties["Model"].Value;
                }
                return hDid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        //4.获取IP地址 
        public static string GetIpAddress()
        {
            try
            {
                var st = "";
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// 5.操作系统的登录用户名 
        public static string GetUserName()
        {
            try
            {
                var un = Environment.UserName;
                return un;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }



        //6.获取计算机名
        static string GetComputerName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        ///7 PC类型 
        public static string GetSystemType()
        {
            try
            {
                var st = "";
                var mc = new ManagementClass("Win32_ComputerSystem");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    st = mo["SystemType"].ToString();
                }
                return st;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        ///8.物理内存        
        public static string GetTotalPhysicalMemory()
        {
            try
            {
                var st = "";
                var mc = new ManagementClass("Win32_ComputerSystem");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    st = mo["TotalPhysicalMemory"].ToString();
                }
                return st;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
