using System;
using System.Net;
using System.Web;

namespace Helper.NetWork
{
    /// <summary>
    /// 和IP有关的类
    /// </summary>
    public class IpHelp
    {
        /// <summary>
        /// 将IP地址转为整数形式
        /// </summary>
        /// <returns>整数</returns>
        public static long Ip2Long(IPAddress ip)
        {
            var x = 3;
            long o = 0;
            foreach (var f in ip.GetAddressBytes())
            {
                o += (long)f << 8 * x--;
            }
            return o;
        }

        /// <summary>
        /// 将整数转为IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public static IPAddress Long2Ip(long l)
        {
            var b = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                b[3 - i] = (byte)(l >> 8 * i & 255);
            }
            return new IPAddress(b);
        }


        /// <summary>
        /// 获得客户端IP
        /// </summary>
        public static string ClientIp
        {
            get
            {
                string ip;
                var isErr = false;
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                else
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"];
                if (ip.Length > 15)
                    isErr = true;
                else
                {
                    var temp = ip.Split('.');
                    if (temp.Length == 4)
                    {
                        for (var i = 0; i < temp.Length; i++)
                        {
                            if (temp[i].Length > 3) isErr = true;
                        }
                    }
                    else
                        isErr = true;
                }
                return isErr ? "1.1.1.1" : ip;
            }
        }

        /// <summary>  
        /// 获取远程访问用户的Ip地址  
        /// </summary>  
        /// <returns>返回Ip地址</returns>  
        public string GetLoginIp()
        {
            string loginip;
            //获取服务变量集合,判断发出请求的远程主机的ip地址是否为空    
            if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            {
                //获取发出请求的远程主机的Ip地址  
                loginip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            //判断登记用户是否使用设置代理  
            else if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    //获取代理的服务器Ip地址  
                    loginip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else
                {
                    //获取客户端IP  
                    loginip = HttpContext.Current.Request.UserHostAddress;
                }
            }
            else
            {
                //获取客户端IP  
                loginip = HttpContext.Current.Request.UserHostAddress;
            }
            return loginip;
        }

        /// <summary>  
        /// 获取公网IP  
        /// </summary>  
        /// <returns></returns>  
        public string GetNetIp()
        {
            var tempIp = "";
            try
            {
                var wr = WebRequest.Create("http://city.ip138.com/ip2city.asp");
                var s = wr.GetResponse().GetResponseStream();
                if (s != null)
                {
                    var sr = new System.IO.StreamReader(s, System.Text.Encoding.GetEncoding("gb2312"));
                    var all = sr.ReadToEnd(); //读取网站的数据  
                    int start = all.IndexOf("[", StringComparison.Ordinal) + 1;
                    int end = all.IndexOf("]", start, StringComparison.Ordinal);
                    tempIp = all.Substring(start, end - start);
                    sr.Close();
                }
                if (s != null) s.Close();
            }
            catch
            {
                if (System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Length > 1)
                    tempIp = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString();
                if (string.IsNullOrEmpty(tempIp))
                    return GetIp();
            }
            return tempIp;
        }

        /// <summary>  
        /// 获取客户端IP地址  
        /// </summary>  
        /// <returns></returns>  
        public  string GetIp()
        {
            var result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return string.IsNullOrEmpty(result) ? "0.0.0.0" : result;
        }


        /// <summary>
        /// 比较两个IP地址是否是同一个IP段
        /// </summary>
        /// <param name="addr1">要比较的IP地址1</param>
        /// <param name="addr2">要比较的IP地址2</param>
        /// <returns>true为相同,false为不同</returns>
        public static bool IsSameIpSectionAs(IPAddress addr1, IPAddress addr2)
        {
            byte[] a1 = addr1.GetAddressBytes();
            byte[] a2 = addr2.GetAddressBytes();

            for (int i = 0; i < a1.Length - 1; i++)
            {
                if (a1[i] != a2[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 比较两个IP地址是否是同一个IP
        /// </summary>
        /// <param name="addr1">要比较的IP地址1</param>
        /// <param name="addr2">要比较的IP地址2</param>
        /// <returns>true为相同,false为不同</returns>
        public static bool IsSameIpAs( IPAddress addr1, IPAddress addr2)
        {
            byte[] a1 = addr1.GetAddressBytes();
            byte[] a2 = addr2.GetAddressBytes();

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }

            return true;
        }


        /// <summary>
        /// 判断两个IP是否属于相同的IP段
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsSameIpBlock(uint ip1, uint ip2)
        {
            return ((ip1 & 0xFFFFFF00) == (ip2 & 0xFFFFFF00));
        }

        /// <summary>
        /// 判断一个IP地址是否属于IP段中的
        /// </summary>
        /// <param name="ip1">IP地址</param>
        /// <param name="ipd">IP段</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsInIpBlock(uint ip1, uint ipd)
        {
            return ((ip1 & 0xFFFFFF00) == ipd);
        }

        /// <summary>
        /// 编码IP
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="calcAll"></param>
        /// <returns><see cref="T:System.Int32"/></returns>
        /// <remarks></remarks>
        public static uint IpGen(IPAddress ip, bool calcAll)
        {
            byte[] tip = ip.GetAddressBytes();

            if (calcAll)
            {
                return (((uint)(tip[0])) << 24) + (((uint)(tip[1])) << 16) + (((uint)(tip[2])) << 8) + ((uint)(tip[3]));
            }
            return (((uint)(tip[0])) << 24) + (((uint)(tip[1])) << 16) + (((uint)(tip[2])) << 8);
        }
        /// <summary>
        /// 反编码IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string IpDisGen(uint ip)
        {
            uint ip1 = (ip & 0xFF000000) >> 24;
            uint ip2 = (ip & 0x00FF0000) >> 16;
            uint ip3 = (ip & 0x0000FF00) >> 8;
            uint ip4 = (ip & 0x000000FF);

            return string.Format("{0}.{1}.{2}.{3}", ip1, ip2, ip3, ip4 == 0 ? "*" : ip4.ToString());
        }
    }
}