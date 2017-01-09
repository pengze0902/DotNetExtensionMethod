using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BasicMmethodExtensionWeb.Helper
{
    public enum MailFormat
    {
        Text,
        Html
    };

    public enum MailPriority
    {
        Low = 1,
        Normal = 3,
        High = 5
    };

    public class MailAttachments
    {
        private const int MaxAttachmentNum = 10;

        private readonly IList _attachments;

        public MailAttachments()
        {
            _attachments = new ArrayList();
        }

        public string this[int index]
        {
            get { return (string)_attachments[index]; }
        }

        /// <summary>
        /// 添加邮件附件
        /// </summary>
        /// <param name="filePath">附件的绝对路径</param>
        public void Add(params string[] filePath)
        {
            if (filePath == null)
            {
                throw (new ArgumentNullException("非法的附件"));
            }
            for (var i = 0; i < filePath.Length; i++)
            {
                Add(filePath[i]);
            }
        }

        /// <summary>
        /// 添加一个附件,当指定的附件不存在时，忽略该附件，不产生异常。
        /// </summary>
        /// <param name="filePath">附件的绝对路径</param>
        public void Add(string filePath)
        {
            //当附件存在时才加入,否则忽略
            if (!File.Exists(filePath)) return;
            if (_attachments.Count < MaxAttachmentNum)
            {
                _attachments.Add(filePath);
            }
        }

        public void Clear()//清除所有附件
        {
            _attachments.Clear();
        }

        public int Count//获取附件个数
        {
            get { return _attachments.Count; }
        }

    }

    /// <summary>
    /// MailMessage 表示SMTP要发送的一封邮件的消息。
    /// </summary>
    public class MailMessage
    {
        public MailMessage()
        {
            Recipients = new ArrayList();//收件人列表
            Attachments = new MailAttachments();//附件
            BodyFormat = MailFormat.Html;//缺省的邮件格式为HTML
            Priority = MailPriority.Normal;
            Charset = "GB2312";
            MaxRecipientNum = 30;
        }

        /// <summary>
        /// 设定语言代码，默认设定为GB2312，如不需要可设置为""
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// 最大收件人
        /// </summary>
        public int MaxRecipientNum { get; set; }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public MailAttachments Attachments { get; set; }

        /// <summary>
        /// 优先权
        /// </summary>
        public MailPriority Priority { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public IList Recipients { get; set; }

        /// <summary>
        /// 邮件格式
        /// </summary>
        public MailFormat BodyFormat { set; get; }

        /// <summary>
        /// 增加一个收件人地址
        /// </summary>
        /// <param name="recipient">收件人的Email地址</param>
        public void AddRecipients(string recipient)
        {
            //先检查邮件地址是否符合规范
            if (Recipients.Count < MaxRecipientNum)
            {
                Recipients.Add(recipient);//增加到收件人列表
            }
        }

        public void AddRecipients(params string[] recipient)
        {
            if (recipient == null)
            {
                throw (new ArgumentException("收件人不能为空."));
            }
            for (int i = 0; i < recipient.Length; i++)
            {
                AddRecipients(recipient[i]);
            }
        }
    }
    public class SmtpServerHelper
    {
        /// <summary>
        /// 回车换行
        /// </summary>
        private string CRLF = "\r\n";

        /// <summary>
        /// 错误消息反馈
        /// </summary>
        public string ErrMsg { set; get; }

        /// <summary>
        /// TcpClient对象，用于连接服务器
        /// </summary> 
        private TcpClient _tcpClient;

        /// <summary>
        /// NetworkStream对象
        /// </summary> 
        private NetworkStream _networkStream;

        /// <summary>
        /// 服务器交互记录
        /// </summary>
        private string _logs = string.Empty;

        /// <summary>
        /// SMTP错误代码哈希表
        /// </summary>
        private readonly Hashtable _errCodeHt = new Hashtable();

        /// <summary>
        /// SMTP正确代码哈希表
        /// </summary>
        private readonly Hashtable _rightCodeHt = new Hashtable();

        /// <summary>
        /// 初始化SMTPCode
        /// </summary>
        public SmtpServerHelper()
        {
            SmtpCodeAdd();
        }

        ~SmtpServerHelper()
        {
            _networkStream.Close();
            _tcpClient.Close();
        }

        /// <summary>
        /// 将Base64字符串解码为普通字符串
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        private string Base64Decode(string str)
        {
            var barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }

        /// <summary>
        /// 得到上传附件的文件流
        /// </summary>
        /// <param name="filePath">附件的绝对路径</param>
        private string GetStream(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(filePath);
            }
            FileStream fileStr = null;
            byte[] by;
            try
            {
                fileStr = new FileStream(filePath, FileMode.Open);
                by = new byte[Convert.ToInt32(fileStr.Length)];
                fileStr.Read(by, 0, by.Length);
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStr != null)
                {
                    fileStr.Close();
                }
            }
            return (Convert.ToBase64String(by));
        }

        /// <summary>
        /// SMTP回应代码哈希表
        /// </summary>
        private void SmtpCodeAdd()
        {
            _errCodeHt.Add("421", "服务未就绪，关闭传输信道");
            _errCodeHt.Add("432", "需要一个密码转换");
            _errCodeHt.Add("450", "要求的邮件操作未完成，邮箱不可用（例如，邮箱忙）");
            _errCodeHt.Add("451", "放弃要求的操作；处理过程中出错");
            _errCodeHt.Add("452", "系统存储不足，要求的操作未执行");
            _errCodeHt.Add("454", "临时认证失败");
            _errCodeHt.Add("500", "邮箱地址错误");
            _errCodeHt.Add("501", "参数格式错误");
            _errCodeHt.Add("502", "命令不可实现");
            _errCodeHt.Add("503", "服务器需要SMTP验证");
            _errCodeHt.Add("504", "命令参数不可实现");
            _errCodeHt.Add("530", "需要认证");
            _errCodeHt.Add("534", "认证机制过于简单");
            _errCodeHt.Add("538", "当前请求的认证机制需要加密");
            _errCodeHt.Add("550", "要求的邮件操作未完成，邮箱不可用（例如，邮箱未找到，或不可访问）");
            _errCodeHt.Add("551", "用户非本地，请尝试<forward-path>");
            _errCodeHt.Add("552", "过量的存储分配，要求的操作未执行");
            _errCodeHt.Add("553", "邮箱名不可用，要求的操作未执行（例如邮箱格式错误）");
            _errCodeHt.Add("554", "传输失败");
            _rightCodeHt.Add("220", "服务就绪");
            _rightCodeHt.Add("221", "服务关闭传输信道");
            _rightCodeHt.Add("235", "验证成功");
            _rightCodeHt.Add("250", "要求的邮件操作完成");
            _rightCodeHt.Add("251", "非本地用户，将转发向<forward-path>");
            _rightCodeHt.Add("334", "服务器响应验证Base64字符串");
            _rightCodeHt.Add("354", "开始邮件输入，以<CRLF>.<CRLF>结束");

        }

            
        /// <summary>
        /// 与服务器交互，发送一条命令并接收回应。
        /// </summary>
        /// <param name="str">一个要发送的命令</param>
        /// <param name="errstr">如果错误，要反馈的信息</param>
        private bool Dialog(string str, string errstr)
        {
            if (str == null || str.Trim() == string.Empty)
            {
                return true;
            }
            if (!SendCommand(str)) return false;
            var rr = RecvResponse();
            if (rr == "false")
            {
                return false;
            }
            //检查返回的代码，根据[RFC 821]返回代码为3位数字代码如220
            var rrCode = rr.Substring(0, 3);
            if (_rightCodeHt[rrCode] != null)
            {
                return true;
            }
            if (_errCodeHt[rrCode] != null)
            {
                ErrMsg += (rrCode + _errCodeHt[rrCode]);
                ErrMsg += CRLF;
            }
            else
            {
                ErrMsg += rr;
            }
            ErrMsg += errstr;
            return false;
        }

        /// <summary>
        /// 发送SMTP命令
        /// </summary> 
        private bool SendCommand(string str)
        {
            if (str == null || str.Trim() == string.Empty)
            {
                return true;
            }
            _logs += str;
            var writeBuffer = Encoding.Default.GetBytes(str);
            try
            {
                _networkStream.Write(writeBuffer, 0, writeBuffer.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }


        //连接服务器
        private bool Connect(string smtpServer, int port)
        {
            //创建Tcp连接
            try
            {
                _tcpClient = new TcpClient(smtpServer, port);
            }
            catch (Exception e)
            {
                throw e;
            }
            _networkStream = _tcpClient.GetStream();
            //验证网络连接是否正确
            if (_rightCodeHt[RecvResponse().Substring(0, 3)] != null)
            {
                return true;
            }
            ErrMsg = "网络连接失败";
            return false;
        }


        /// <summary>
        /// 接收SMTP服务器回应
        /// </summary>
        private string RecvResponse()
        {
            int streamSize;
            var returnvalue = string.Empty;
            var readBuffer = new byte[1024];
            try
            {
                streamSize = _networkStream.Read(readBuffer, 0, readBuffer.Length);
            }
            catch
            {
                ErrMsg = "网络连接错误";
                return "false";
            }

            if (streamSize == 0)
            {
                return returnvalue;
            }
            returnvalue = Encoding.Default.GetString(readBuffer).Substring(0, streamSize);
            _logs += returnvalue + CRLF;
            return returnvalue;
        }


        private string GetPriorityString(MailPriority mailPriority)
        {
            var priority = "Normal";
            switch (mailPriority)
            {
                case MailPriority.Low:
                    priority = "Low";
                    break;
                case MailPriority.High:
                    priority = "High";
                    break;
            }
            return priority;
        }

        /// <summary>
        /// 发送电子邮件，SMTP服务器不需要身份验证
        /// </summary>
        /// <param name="smtpServer">发信SMTP服务器</param>
        /// <param name="port">端口，默认为25</param>
        /// <param name="mailMessage">邮件内容</param>
        /// <returns></returns>
        public bool SendEmail(string smtpServer, int port, MailMessage mailMessage)
        {
            return SendEmail(smtpServer, port, false, "", "", mailMessage);
        }

        /// <summary>
        /// 发送电子邮件，SMTP服务器需要身份验证
        /// </summary>
        /// <param name="smtpServer">发信SMTP服务器</param>
        /// <param name="port">端口，默认为25</param>
        /// <param name="username">发信人邮箱地址</param>
        /// <param name="password">发信人邮箱密码</param>
        /// <param name="mailMessage">邮件内容</param>
        /// <returns></returns>
        public bool SendEmail(string smtpServer, int port, string username, string password, MailMessage mailMessage)
        {
            return SendEmail(smtpServer, port, true, username, password, mailMessage);
        }

        private bool SendEmail(string smtpServer, int port, bool eSmtp, string username, string password, MailMessage mailMessage)
        {
            //测试连接服务器是否成功
            if (Connect(smtpServer, port) == false)
                return false;

            var priority = GetPriorityString(mailMessage.Priority);
            var html = (mailMessage.BodyFormat == MailFormat.Html);
            string[] sendBuffer;
            string sendBufferstr;

            //进行SMTP验证，现在大部分SMTP服务器都要认证
            if (eSmtp)
            {
                sendBuffer = new string[4];
                sendBuffer[0] = "EHLO " + smtpServer + CRLF;
                sendBuffer[1] = "AUTH LOGIN" + CRLF;
                sendBuffer[2] = Base64Encode(username) + CRLF;
                sendBuffer[3] = Base64Encode(password) + CRLF;
                if (!Dialog(sendBuffer, "SMTP服务器验证失败，请核对用户名和密码。"))
                    return false;
            }
            else
            {
                //不需要身份认证
                sendBufferstr = "HELO " + smtpServer + CRLF;
                if (!Dialog(sendBufferstr, ""))
                    return false;
            }

            //发件人地址
            sendBufferstr = "MAIL FROM:<" + username + ">" + CRLF;
            if (!Dialog(sendBufferstr, "发件人地址错误，或不能为空"))
                return false;

            //收件人地址
            sendBuffer = new string[mailMessage.Recipients.Count];
            for (var i = 0; i < mailMessage.Recipients.Count; i++)
            {
                sendBuffer[i] = "RCPT TO:<" + (string)mailMessage.Recipients[i] + ">" + CRLF;
            }

            if (!Dialog(sendBuffer, "收件人地址有误"))
                return false;

            sendBufferstr = "DATA" + CRLF;
            if (!Dialog(sendBufferstr, ""))
                return false;

            //发件人姓名
            sendBufferstr = "From:" + mailMessage.FromName + "<" + mailMessage.From + ">" + CRLF;

            if (mailMessage.Recipients.Count == 0)
            {
                return false;
            }
            sendBufferstr += "To:=?" + mailMessage.Charset.ToUpper() + "?B?" +
                             Base64Encode((string)mailMessage.Recipients[0]) + "?=" + "<" + (string)mailMessage.Recipients[0] + ">" + CRLF;

            sendBufferstr +=
             (string.IsNullOrEmpty(mailMessage.Subject) ? "Subject:" : ((mailMessage.Charset == "") ? ("Subject:" +
             mailMessage.Subject) : ("Subject:" + "=?" + mailMessage.Charset.ToUpper() + "?B?" +
             Base64Encode(mailMessage.Subject) + "?="))) + CRLF;
            sendBufferstr += "X-Priority:" + priority + CRLF;
            sendBufferstr += "X-MSMail-Priority:" + priority + CRLF;
            sendBufferstr += "Importance:" + priority + CRLF;
            sendBufferstr += "X-Mailer: Lion.Web.Mail.SmtpMail Pubclass [cn]" + CRLF;
            sendBufferstr += "MIME-Version: 1.0" + CRLF;
            if (mailMessage.Attachments.Count != 0)
            {
                sendBufferstr += "Content-Type: multipart/mixed;" + CRLF;
                sendBufferstr += " boundary=\"=====" +
                 (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====\"" + CRLF + CRLF;
            }

            if (html)
            {
                if (mailMessage.Attachments.Count == 0)
                {
                    sendBufferstr += "Content-Type: multipart/alternative;" + CRLF;//内容格式和分隔符
                    sendBufferstr += " boundary=\"=====003_Dragon520636771063_=====\"" + CRLF + CRLF;
                    sendBufferstr += "This is a multi-part message in MIME format." + CRLF + CRLF;
                }
                else
                {
                    sendBufferstr += "This is a multi-part message in MIME format." + CRLF + CRLF;
                    sendBufferstr += "--=====001_Dragon520636771063_=====" + CRLF;
                    sendBufferstr += "Content-Type: multipart/alternative;" + CRLF;//内容格式和分隔符
                    sendBufferstr += " boundary=\"=====003_Dragon520636771063_=====\"" + CRLF + CRLF;
                }
                sendBufferstr += "--=====003_Dragon520636771063_=====" + CRLF;
                sendBufferstr += "Content-Type: text/plain;" + CRLF;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" +

                 mailMessage.Charset.ToLower() + "\"")) + CRLF;
                sendBufferstr += "Content-Transfer-Encoding: base64" + CRLF + CRLF;
                sendBufferstr += Base64Encode("邮件内容为HTML格式，请选择HTML方式查看") + CRLF + CRLF;

                sendBufferstr += "--=====003_Dragon520636771063_=====" + CRLF;


                sendBufferstr += "Content-Type: text/html;" + CRLF;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" +
                 mailMessage.Charset.ToLower() + "\"")) + CRLF;
                sendBufferstr += "Content-Transfer-Encoding: base64" + CRLF + CRLF;
                sendBufferstr += Base64Encode(mailMessage.Body) + CRLF + CRLF;
                sendBufferstr += "--=====003_Dragon520636771063_=====--" + CRLF;
            }
            else
            {
                if (mailMessage.Attachments.Count != 0)
                {
                    sendBufferstr += "--=====001_Dragon303406132050_=====" + CRLF;
                }
                sendBufferstr += "Content-Type: text/plain;" + CRLF;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" +
                 mailMessage.Charset.ToLower() + "\"")) + CRLF;
                sendBufferstr += "Content-Transfer-Encoding: base64" + CRLF + CRLF;
                sendBufferstr += Base64Encode(mailMessage.Body) + CRLF;
            }

            if (mailMessage.Attachments.Count != 0)
            {
                for (int i = 0; i < mailMessage.Attachments.Count; i++)
                {
                    var filepath = mailMessage.Attachments[i];
                    sendBufferstr += "--=====" +
                     (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====" + CRLF;
                    sendBufferstr += "Content-Type: text/plain;" + CRLF;
                    sendBufferstr += " name=\"=?" + mailMessage.Charset.ToUpper() + "?B?" +
                     Base64Encode(filepath.Substring(filepath.LastIndexOf("\\", StringComparison.Ordinal) + 1)) + "?=\"" + CRLF;
                    sendBufferstr += "Content-Transfer-Encoding: base64" + CRLF;
                    sendBufferstr += "Content-Disposition: attachment;" + CRLF;
                    sendBufferstr += " filename=\"=?" + mailMessage.Charset.ToUpper() + "?B?" +
                     Base64Encode(filepath.Substring(filepath.LastIndexOf("\\", StringComparison.Ordinal) + 1)) + "?=\"" + CRLF + CRLF;
                    sendBufferstr += GetStream(filepath) + CRLF + CRLF;
                }
                sendBufferstr += "--=====" +
                 (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====--" + CRLF + CRLF;
            }

            sendBufferstr += CRLF + "." + CRLF;//内容结束

            if (!Dialog(sendBufferstr, "错误信件信息"))
                return false;

            sendBufferstr = "QUIT" + CRLF;
            if (!Dialog(sendBufferstr, "断开连接时错误"))
                return false;

            _networkStream.Close();
            _tcpClient.Close();
            return true;
        }

        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        private static string Base64Encode(string str)
        {
            var barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        /// 与服务器交互，发送一组命令并接收回应。
        /// </summary>
        private bool Dialog(IReadOnlyList<string> str, string errstr)
        {
            for (var i = 0; i < str.Count; i++)
            {
                if (Dialog(str[i], "")) continue;
                ErrMsg += CRLF;
                ErrMsg += errstr;
                return false;
            }
            return true;
        }
    }



    public class SmtpClient
    {
        public SmtpClient()
        {
        }
        public SmtpClient(string smtpServer, int smtpPort)
        {
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
        }

        /// <summary>
        /// 错误消息反馈
        /// </summary>
        public string ErrMsg { get; private set; }

        public string SmtpServer { set; get; }

        public int SmtpPort { set; get; }

        public bool Send(MailMessage mailMessage, string username, string password)
        {
            var helper = new SmtpServerHelper();
            if (helper.SendEmail(SmtpServer, SmtpPort, username, password, mailMessage))
                return true;
            ErrMsg = helper.ErrMsg;
            return false;
        }

    }
}


