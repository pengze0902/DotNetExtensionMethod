using System;

namespace BasicMmethodExtensionWeb.Helper
{
    /// <summary>
    /// Log4Net日志封装类
    /// </summary>
    public class Log4Helper
    {
        /// <summary>
        /// 信息标志
        /// </summary>
        private static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("loginfo");

        /// <summary>
        /// 错误标志
        /// </summary>
        private static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        /// 调试标志
        /// </summary>
        private static readonly log4net.ILog Logdebug = log4net.LogManager.GetLogger("logdebug");


        /// <summary>
        /// Log4Net信息记录封装
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void Info(string message)
        {
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(message);
            }
        }

        /// <summary>
        /// Log4Net错误记录封装
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void Error(string message)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(message);
            }
        }

        /// <summary>
        /// Log4Net错误记录封装
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static void Error(string message, Exception ex)
        {
            if (Logerror.IsErrorEnabled)
            {
                if (!string.IsNullOrEmpty(message) && ex == null)
                {
                    Logerror.ErrorFormat("<br/>【附加信息】 : {0}<br>", new object[] { message });
                }
                else if (!string.IsNullOrEmpty(message) && ex != null)
                {
                    string errorMsg = BeautyErrorMsg(ex);
                    Logerror.ErrorFormat("<br/>【附加信息】 : {0}<br>{1}", new object[] { message, errorMsg });
                }
                else if (string.IsNullOrEmpty(message) && ex != null)
                {
                    string errorMsg = BeautyErrorMsg(ex);
                    Logerror.Error(errorMsg);
                }
            }
        }


        /// <summary>
        /// Log4Net调试记录封装
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void Debug(string message)
        {
            if (Logdebug.IsErrorEnabled)
            {
                Logdebug.Debug(message);
            }
        }

        /// <summary>
        /// Log4Net调试记录封装 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static void Debug(string message, Exception ex)
        {
            if (Logdebug.IsDebugEnabled)
            {
                if (!string.IsNullOrEmpty(message) && ex == null)
                {
                    Logdebug.DebugFormat("<br/>【附加信息】 : {0}<br>", new object[] { message });
                }
                else if (!string.IsNullOrEmpty(message) && ex != null)
                {
                    string errorMsg = BeautyErrorMsg(ex);
                    Logdebug.DebugFormat("<br/>【附加信息】 : {0}<br>{1}", new object[] { message, errorMsg });
                }
                else if (string.IsNullOrEmpty(message) && ex != null)
                {
                    string errorMsg = BeautyErrorMsg(ex);
                    Logdebug.Debug(errorMsg);
                }
            }
        }

        /// <summary>
        /// 美化错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>错误信息</returns>
        private static string BeautyErrorMsg(Exception ex)
        {
            string errorMsg = string.Format("【异常类型】：{0} <br>【异常信息】：{1} <br>【堆栈调用】：{2}",
                new object[] { ex.GetType().Name, ex.Message, ex.StackTrace });
            errorMsg = errorMsg.Replace("\r\n", "<br>");
            errorMsg = errorMsg.Replace("位置", "<strong style=\"color:red\">位置</strong><br/>");
            return errorMsg;
        }
    }
}
