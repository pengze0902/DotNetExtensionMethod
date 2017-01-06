using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace BasicMmethodExtensionWeb.Helper
{
    public class ConfigHelper
    {
        private ConfigHelper()
        { }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">节点名称</param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 设置配置值（存在则更新，不存在则新增）
        /// 在新增appSettings节点时，不会写入App.config或web.config中，因为AppSetting这样的节点属于内置节点，
        /// 会存储在Machine.config文件中
        /// </summary>
        /// <param name="key">节点名称</param>
        /// <param name="value">节点值</param>
        public static void SetAppSettingValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var setting = config.AppSettings.Settings[key];
                if (setting == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    setting.Value = value;
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (ConfigurationErrorsException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除配置值
        /// </summary>
        /// <param name="key">节点名称</param>
        public static void RemoveAppSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (ConfigurationErrorsException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        /// <summary>
        /// 设置连接字符串的值（存在则更新，不存在则新增）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="connstr">连接字符串</param>
        /// <param name="provider">程序名称属性</param>
        public static void SetConnectionString(string name, string connstr, string provider)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            if (string.IsNullOrEmpty(connstr))
            {
                throw new ArgumentNullException(connstr);
            }
            if (string.IsNullOrEmpty(provider))
            {
                throw new ArgumentNullException(provider);
            }
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connStrSettings = config.ConnectionStrings.ConnectionStrings[name];
                if (connStrSettings != null)
                {
                    connStrSettings.ConnectionString = connstr;
                    connStrSettings.ProviderName = provider;
                }
                else
                {
                    connStrSettings = new ConnectionStringSettings(name, connstr, provider);
                    config.ConnectionStrings.ConnectionStrings.Add(connStrSettings);
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (ConfigurationErrorsException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除连接字符串配置项
        /// </summary>
        /// <param name="name">字符串名称</param>
        public static void RemoveConnectionString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.ConnectionStrings.ConnectionStrings.Remove(name);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (ConfigurationErrorsException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取节点的所有属性（处理单个节点）
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>属性的Hashtable列表</returns>
        public static Hashtable GetNodeAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            return (Hashtable)ConfigurationManager.GetSection(name);
        }

        /// <summary>
        /// 更新或新增[appSettings]节点的子节点值，存在则更新子节点Value,不存在则新增子节点，返回成功与否布尔值
        /// </summary>
        /// <param name="filename">配置文件的路径</param>
        /// <param name="key">子节点Key值</param>
        /// <param name="value">子节点value值</param>
        /// <returns>返回成功与否布尔值</returns>
        public static bool UpdateOrCreateAppSetting(string filename, string key, string value)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }
            var doc = new XmlDocument();
            //加载配置文件
            doc.Load(filename);
            //得到[appSettings]节点
            var node = doc.SelectSingleNode("//appSettings");
            try
            {
                //得到[appSettings]节点中关于Key的子节点
                if (node != null)
                {
                    var element = (XmlElement)node.SelectSingleNode("//add[@key='" + key + "']");
                    if (element != null)
                    {
                        //存在则更新子节点Value
                        element.SetAttribute("value", value);
                    }
                    else
                    {
                        //不存在则新增子节点
                        var subElement = doc.CreateElement("add");
                        subElement.SetAttribute("key", key);
                        subElement.SetAttribute("value", value);
                        node.AppendChild(subElement);
                    }
                }
                //保存至配置文件(方式一)
                using (var xmlwriter = new XmlTextWriter(filename, null))
                {
                    xmlwriter.Formatting = Formatting.Indented;
                    doc.WriteTo(xmlwriter);
                    xmlwriter.Flush();
                }
            }
            catch (XPathException ex)
            {
                throw ex;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            return true;
        }


        /// <summary>
        /// 更新或新增[connectionStrings]节点的子节点值，存在则更新子节点,不存在则新增子节点，返回成功与否布尔值
        /// </summary>
        /// <param name="filename">配置文件路径</param>
        /// <param name="name">子节点name值</param>
        /// <param name="connectionString">子节点connectionString值</param>
        /// <param name="providerName">子节点providerName值</param>
        /// <returns>返回成功与否布尔值</returns>
        public static bool UpdateOrCreateConnectionString(string filename, string name, string connectionString, string providerName)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(connectionString);
            }
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentNullException(providerName);
            }
            var doc = new XmlDocument();
            //加载配置文件
            doc.Load(filename);
            //得到[connectionStrings]节点
            var node = doc.SelectSingleNode("//connectionStrings");
            try
            {
                //得到[connectionStrings]节点中关于Name的子节点
                if (node != null)
                {
                    XmlElement element = (XmlElement)node.SelectSingleNode("//add[@name='" + name + "']");
                    if (element != null)
                    {
                        //存在则更新子节点
                        element.SetAttribute("connectionString", connectionString);
                        element.SetAttribute("providerName", providerName);
                    }
                    else
                    {
                        //不存在则新增子节点
                        var subElement = doc.CreateElement("add");
                        subElement.SetAttribute("name", name);
                        subElement.SetAttribute("connectionString", connectionString);
                        subElement.SetAttribute("providerName", providerName);
                        node.AppendChild(subElement);
                    }
                }
                //保存至配置文件(方式二)
                doc.Save(filename);
            }
            catch (XPathException ex)
            {
                throw ex;
            }
            catch (XmlException xmex)
            {
                throw xmex;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            return true;
        }


        /// <summary>
        /// 删除[appSettings]节点中包含Key值的子节点，返回成功与否布尔值
        /// </summary>
        /// <param name="filename">配置文件路径</param>
        /// <param name="key">要删除的子节点Key值</param>
        /// <returns>返回成功与否布尔值</returns>
        public static bool DeleteByKey(string filename, string key)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }
            var doc = new XmlDocument();
            //加载配置文件
            doc.Load(filename);
            //得到[appSettings]节点
            var node = doc.SelectSingleNode("//appSettings");
            //得到[appSettings]节点中关于Key的子节点
            if (node != null)
            {
                var element = (XmlElement)node.SelectSingleNode("//add[@key='" + key + "']");
                if (element != null)
                {
                    //存在则删除子节点
                    if (element.ParentNode != null)
                    {
                        element.ParentNode.RemoveChild(element);
                    }
                }
            }
            try
            {
                //保存至配置文件(方式一)
                using (var xmlwriter = new XmlTextWriter(filename, null))
                {
                    xmlwriter.Formatting = Formatting.Indented;
                    doc.WriteTo(xmlwriter);
                    xmlwriter.Flush();
                }
            }
            catch (XPathException ex)
            {
                throw ex;
            }
            catch (XmlException xmex)
            {
                throw xmex;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            catch (IOException ioex)
            {
                throw ioex;
            }
            return true;
        }


        /// <summary>
        /// 删除[connectionStrings]节点中包含name值的子节点，返回成功与否布尔值
        /// </summary>
        /// <param name="filename">配置文件路径</param>
        /// <param name="name">要删除的子节点name值</param>
        /// <returns>返回成功与否布尔值</returns>
        public static bool DeleteByName(string filename, string name)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            var doc = new XmlDocument();
            //加载配置文件
            doc.Load(filename);
            //得到[connectionStrings]节点
            var node = doc.SelectSingleNode("//connectionStrings");
            //得到[connectionStrings]节点中关于Name的子节点
            if (node != null)
            {
                var element = (XmlElement)node.SelectSingleNode("//add[@name='" + name + "']");
                if (element != null)
                {
                    //存在则删除子节点
                    node.RemoveChild(element);
                }
            }
            try
            {
                //保存至配置文件(方式二)
                doc.Save(filename);
            }
            catch (XmlException ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
