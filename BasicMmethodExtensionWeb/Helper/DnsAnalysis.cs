using System;
using System.Diagnostics;
using System.Net;
using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;

namespace BasicMmethodExtensionWeb.Helper
{
    public class DnsAnalysis
    {
        /// <summary>
        /// DNS解析
        /// </summary>
        /// <param name="dnsServer">DNS服务器IP</param>
        /// <param name="timeOut">解析超时时间</param>
        /// <param name="url">解析网址</param>
        /// <param name="isSuccess">是否解析成功</param>
        /// <returns>解析到的IP信息</returns>
        public static IPAddress DnsResolver(string dnsServer, int timeOut, string url, out bool isSuccess)
        {
            if (string.IsNullOrEmpty(dnsServer))
            {
                throw new ArgumentNullException(dnsServer);
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(url);
            }
            //初始化DnsClient，第一个参数为DNS服务器的IP，第二个参数为超时时间
            var dnsClient = new DnsClient(IPAddress.Parse(dnsServer), timeOut);
            //解析域名。将域名请求发送至DNS服务器解析，第一个参数为需要解析的域名，第二个参数为
            //解析类型， RecordType.A为IPV4类型
            //DnsMessage dnsMessage = dnsClient.Resolve("www.sina.com", RecordType.A);
            var s = new Stopwatch();
            s.Start();
            var dnsMessage = dnsClient.Resolve(DomainName.Parse(url));
            s.Stop();
            //若返回结果为空，或者存在错误，则该请求失败。
            if (dnsMessage == null || (dnsMessage.ReturnCode != ReturnCode.NoError && dnsMessage.ReturnCode != ReturnCode.NxDomain))
            {
                isSuccess = false;
            }
            //循环遍历返回结果，将返回的IPV4记录添加到结果集List中。
            if (dnsMessage != null)
                foreach (var dnsRecord in dnsMessage.AnswerRecords)
                {
                    var aRecord = dnsRecord as ARecord;
                    if (aRecord == null) continue;
                    isSuccess = true;
                    return aRecord.Address;
                }
            isSuccess = false;
            return null;
        }
    }
}
