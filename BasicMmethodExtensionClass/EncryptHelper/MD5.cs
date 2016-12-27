namespace Helper.EncryptHelper
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 32位大写
        /// </summary>
        /// <returns></returns>
        public static string Upper32(string s)
        {
            var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");
            if (hashPasswordForStoringInConfigFile != null)
                s = hashPasswordForStoringInConfigFile.ToString();
            return s.ToUpper();
        }

        /// <summary>
        /// 32位小写
        /// </summary>
        /// <returns></returns>
        public static string Lower32(string s)
        {
            var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");
            if (hashPasswordForStoringInConfigFile != null)
                s = hashPasswordForStoringInConfigFile.ToString();
            return s.ToLower();
        }

        /// <summary>
        /// 16位大写
        /// </summary>
        /// <returns></returns>
        public static string Upper16(string s)
        {
            var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");
            if (hashPasswordForStoringInConfigFile != null)
                s = hashPasswordForStoringInConfigFile.ToString();
            return s.ToUpper().Substring(8, 16);
        }

        /// <summary>
        /// 16位小写
        /// </summary>
        /// <returns></returns>
        public static string Lower16(string s)
        {
            var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");
            if (hashPasswordForStoringInConfigFile != null)
                s = hashPasswordForStoringInConfigFile.ToString();
            return s.ToLower().Substring(8, 16);
        }
    }
}
