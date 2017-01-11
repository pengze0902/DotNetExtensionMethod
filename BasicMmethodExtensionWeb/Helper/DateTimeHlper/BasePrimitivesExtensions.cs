using System.Globalization;

namespace BasicMmethodExtensionWeb.Helper.DateTimeHlper
{
    /// <summary>
    ///基本扩展默认数据
    /// </summary>
    public class BasePrimitivesExtensions
    {
        //Boolean extensions
        private const bool DefaultBoolConversion = false;
        private const bool DefaultBoolArrayAllowDefaultConversion = false;

        //Byte extensions
        private const NumberStyles DefaultByteNumberStyle = NumberStyles.None;
        private const byte DefaultByteConversion = 0;
        private const bool DefaultByteAllowDefaultConversion = true;
        private const bool DefaultByteArrayAllowDefaultConversion = true;

        //Int16 extensions
        private const NumberStyles DefaultShortNumberStyle = NumberStyles.Integer;
        private const short DefaultShortConversion = 0;
        private const bool DefaultShortAllowDefaultConversion = true;
        private const bool DefaultShortArrayAllowDefaultConversion = true;

        //Int32 extensions
        private const NumberStyles DefaultIntNumberStyle = NumberStyles.Integer;
        private const int DefaultIntConversion = 0;
        private const bool DefaultIntAllowDefaultConversion = true;
        private const bool DefaultIntArrayAllowDefaultConversion = true;

        //Int64 extensions
        private const NumberStyles DefaultLongNumberStyle = NumberStyles.Integer;
        private const long DefaultLongConversion = 0;
        private const bool DefaultLongAllowDefaultConversion = true;
        private const bool DefaultLongArrayAllowDefaultConversion = true;

        //Decimal extensions
        private const NumberStyles DefaultDecimalNumberStyle = NumberStyles.Any;
        private const decimal DefaultDecimalConversion = 0;
        private const bool DefaultDecimalAllowDefaultConversion = true;
        private const bool DefaultDecimalArrayAllowDefaultConversion = true;

        //Double extensions
        private const NumberStyles DefaultDoubleNumberStyle = NumberStyles.Any;
        private const double DefaultDoubleConversion = 0;
        private const bool DefaultDoubleAllowDefaultConversion = true;
        private const bool DefaultDoubleArrayAllowDefaultConversion = true;

        //Float extensions
        private const NumberStyles DefaultFloatNumberStyle = NumberStyles.Any;
        private const float DefaultFloatConversion = 0;
        private const bool DefaultFloatAllowDefaultConversion = true;
        private const bool DefaultFloatArrayAllowDefaultConversion = true; 

        /// <summary>
        /// 获取默认的bool转换值
        /// </summary>
        /// <returns>DefaultBoolConversion</returns>
        public static bool GetDefaultBoolConversionValue()
        {
            return DefaultBoolConversion;
        }

        /// <summary>
        /// 获取默认bool数组允许默认转换值
        /// </summary>
        /// <returns>DefaultBoolArrayAllowDefaultConversion</returns>
        public static bool GetDefaultBoolArrayAllowDefaultConversion()
        {
            return DefaultBoolArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认字节数样式
        /// </summary>
        /// <returns>Byte NumberStyles</returns>
        public static NumberStyles GetDefaultByteNumberStyle()
        {
            return DefaultByteNumberStyle;
        }

        /// <summary>
        /// 获取默认字节转换值
        /// </summary>
        /// <returns>DefaultByteConversion</returns>
        public static byte GetDefaultByteConversionValue()
        {
            return DefaultByteConversion;
        }

        /// <summary>
        /// 获取默认字节允许默认转换
        /// </summary>
        /// <returns>DefaultByteAllowDefaultConversion</returns>
        public static bool GetDefaultByteAllowDefaultConversion()
        {
            return DefaultByteAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认字节数组允许默认转换值
        /// </summary>
        /// <returns>DefaultByteArrayAllowDefaultConversion</returns>
        public static bool GetDefaultByteArrayAllowDefaultConversion()
        {
            return DefaultByteArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认的短号样式
        /// </summary>
        /// <returns>Short NumberStyles</returns>
        public static NumberStyles GetDefaultShortNumberStyle()
        {
            return DefaultShortNumberStyle;
        }

        /// <summary>
        /// 获取默认的短转换值
        /// </summary>
        /// <returns>DefaultShortConversion</returns>
        public static short GetDefaultShortConversionValue()
        {
            return DefaultShortConversion;
        }

        /// <summary>
        /// 获取默认short允许默认转换
        /// </summary>
        /// <returns>DefaultShortAllowDefaultConversion</returns>
        public static bool GetDefaultShortAllowDefaultConversion()
        {
            return DefaultShortAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认排序数组允许默认转换值
        /// </summary>
        /// <returns>DefaultShortArrayAllowDefaultConversion</returns>
        public static bool GetDefaultShortArrayAllowDefaultConversion()
        {
            return DefaultShortArrayAllowDefaultConversion;
        }

        /// <summary>
        ///获取默认的int数值样式
        /// </summary>
        /// <returns>Int NumberStyles</returns>
        public static NumberStyles GetDefaultIntNumberStyle()
        {
            return DefaultIntNumberStyle;
        }

        /// <summary>
        ///获取默认int转换值
        /// </summary>
        /// <returns>DefaultIntConversion</returns>
        public static int GetDefaultIntConversionValue()
        {
            return DefaultIntConversion;
        }

        /// <summary>
        /// 获取默认int允许默认转换
        /// </summary>
        /// <returns>DefaultIntAllowDefaultConversion</returns>
        public static bool GetDefaultIntAllowDefaultConversion()
        {
            return DefaultIntAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认int数组允许默认转换值
        /// </summary>
        /// <returns>DefaultIntArrayAllowDefaultConversion</returns>
        public static bool GetDefaultIntArrayAllowDefaultConversion()
        {
            return DefaultIntArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认长数字样式
        /// </summary>
        /// <returns>Long NumberStyles</returns>
        public static NumberStyles GetDefaultLongNumberStyle()
        {
            return DefaultLongNumberStyle;
        }

        /// <summary>
        /// 获取默认的长转换价值
        /// </summary>
        /// <returns>DefaultDecimalConversion</returns>
        public static long GetDefaultLongConversionValue()
        {
            return DefaultLongConversion;
        }

        /// <summary>
        /// 获取默认长允许默认转换
        /// </summary>
        /// <returns>DefaultLongAllowDefaultConversion</returns>
        public static bool GetDefaultLongAllowDefaultConversion()
        {
            return DefaultLongAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认长阵列允许默认转换值
        /// </summary>
        /// <returns>DefaultLongArrayAllowDefaultConversion</returns>
        public static bool GetDefaultLongArrayAllowDefaultConversion()
        {
            return DefaultLongArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认十进制数字样式
        /// </summary>
        /// <returns>Decimal NumberStyles</returns>
        public static NumberStyles GetDefaultDecimalNumberStyle()
        {
            return DefaultDecimalNumberStyle;
        }

        /// <summary>
        ///获取默认十进制转换值
        /// </summary>
        /// <returns>DefaultDecimalConversion</returns>
        public static decimal GetDefaultDecimalConversionValue()
        {
            return DefaultDecimalConversion;
        }

        /// <summary>
        /// 获取默认十进制允许默认转换
        /// </summary>
        /// <returns>DefaultDecimalAllowDefaultConversion</returns>
        public static bool GetDefaultDecimalAllowDefaultConversion()
        {
            return DefaultDecimalAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认十进制数组允许默认转换值
        /// </summary>
        /// <returns>DefaultDecimalArrayAllowDefaultConversion</returns>
        public static bool GetDefaultDecimalArrayAllowDefaultConversion()
        {
            return DefaultDecimalArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认双精度数字样式
        /// </summary>
        /// <returns>Double NumberStyles</returns>
        public static NumberStyles GetDefaultDoubleNumberStyle()
        {
            return DefaultDoubleNumberStyle;
        }

        /// <summary>
        /// 获取默认的双精度转换值
        /// </summary>
        /// <returns>DefaultDoubleConversion</returns>
        public static double GetDefaultDoubleConversionValue()
        {
            return DefaultDoubleConversion;
        }

        /// <summary>
        /// 获取默认双精度允许默认转换
        /// </summary>
        /// <returns>DefaultDoubleAllowDefaultConversion</returns>
        public static bool GetDefaultDoubleAllowDefaultConversion()
        {
            return DefaultDoubleAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认的Double数组允许默认转换值
        /// </summary>
        /// <returns>DefaultDoubleArrayAllowDefaultConversion</returns>
        public static bool GetDefaultDoubleArrayAllowDefaultConversion()
        {
            return DefaultDoubleArrayAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认浮点数样式
        /// </summary>
        /// <returns>Float NumberStyles</returns>
        public static NumberStyles GetDefaultFloatNumberStyle()
        {
            return DefaultFloatNumberStyle;
        }

        /// <summary>
        /// 获取默认浮动广告转换值
        /// </summary>
        /// <returns>DefaultFloatConversion</returns>
        public static float GetDefaultFloatConversionValue()
        {
            return DefaultFloatConversion;
        }

        /// <summary>
        /// 获取默认float允许默认转换
        /// </summary>
        /// <returns>DefaultFloatAllowDefaultConversion</returns>
        public static bool GetDefaultFloatAllowDefaultConversion()
        {
            return DefaultFloatAllowDefaultConversion;
        }

        /// <summary>
        /// 获取默认float数组允许默认转换值
        /// </summary>
        /// <returns>DefaultFloatArrayAllowDefaultConversion</returns>
        public static bool GetDefaultFloatArrayAllowDefaultConversion()
        {
            return DefaultFloatArrayAllowDefaultConversion;
        }

        /// <summary>
        ///获取当前的系统文化
        /// </summary>
        /// <returns>Current CultureInfo</returns>
        public static CultureInfo GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }
    }
}
