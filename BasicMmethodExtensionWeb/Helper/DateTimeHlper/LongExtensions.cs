using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace BasicMmethodExtensionWeb.Helper.DateTimeHlper
{
    /// <summary>
    /// 静态长整型扩展
    /// </summary>
    public static class LongExtensions
    {
        /// <summary>
        ///将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <param name="defaultValue">default value when error on convert value</param>
        /// <param name="allowZero">allow zero on convert</param>
        /// <param name="numberStyle">string number style</param>
        /// <param name="culture">current culture</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue, long defaultValue, bool allowZero, NumberStyles numberStyle, CultureInfo culture)
        {
            long longValue;
            var converted = long.TryParse(strValue, numberStyle, culture, out longValue);

            return converted ? longValue == 0 && !allowZero ? defaultValue : longValue : defaultValue;
        }

        /// <summary>
        /// 将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <param name="defaultValue">default value when error on convert value</param>
        /// <param name="numberStyle">string number style</param>
        /// <param name="culture">current culture</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue, long defaultValue, NumberStyles numberStyle, CultureInfo culture)
        {
            return strValue.TryParseLong(defaultValue,
                BasePrimitivesExtensions.GetDefaultLongAllowDefaultConversion(),
                numberStyle, culture);
        }

        /// <summary>
        /// 将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <param name="numberStyle">string number style</param>
        /// <param name="culture">current culture</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue, NumberStyles numberStyle, CultureInfo culture)
        {
            return strValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue(),
                BasePrimitivesExtensions.GetDefaultLongAllowDefaultConversion(),
                numberStyle, culture);
        }

        /// <summary>
        /// 将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <param name="defaultValue">default value when error on convert value</param>
        /// <param name="allowZero">allow zero on convert</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue, long defaultValue, bool allowZero)
        {
            return strValue.TryParseLong(defaultValue, allowZero,
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        /// 将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <param name="defaultValue">default value when error on convert value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue, long defaultValue)
        {
            return strValue.TryParseLong(defaultValue,
                BasePrimitivesExtensions.GetDefaultLongAllowDefaultConversion(),
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        /// 将字符串值转换为长整型值
        /// </summary>
        /// <param name="strValue">string value to convert</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this string strValue)
        {
            return strValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue(),
                BasePrimitivesExtensions.GetDefaultLongAllowDefaultConversion(),
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        /// 将nullable长到长
        /// </summary>
        /// <param name="longValue">nullable long value</param>
        /// <param name="defaultValue">default value when error on convert</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this long? longValue, long defaultValue)
        {
            return longValue == null ? defaultValue : Convert.ToInt64(longValue);
        }

        /// <summary>
        /// 将nullable长到长
        /// </summary>
        /// <param name="longValue">nullable long value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this long? longValue)
        {
            return longValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将字节值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="byteValue">long value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this byte byteValue)
        {
            return Convert.ToInt64(byteValue);
        }

        /// <summary>
        /// <para>将可空字节值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="byteValue">nullable byte value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this byte? byteValue, long defaultValue)
        {
            return byteValue == null ? defaultValue : Convert.ToInt64(byteValue);
        }

        /// <summary>
        /// 将可空字节值转换为长整型值
        /// </summary>
        /// <param name="byteValue">nullable byte value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this byte? byteValue)
        {
            return byteValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将短值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="shortValue">short value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this short shortValue)
        {
            return Convert.ToInt64(shortValue);
        }

        /// <summary>
        /// <para>将可空短值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="shortValue">nullable short value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this short? shortValue, long defaultValue)
        {
            return shortValue == null ? defaultValue : Convert.ToInt64(shortValue);
        }

        /// <summary>
        /// 将可空短值转换为长整型值
        /// </summary>
        /// <param name="shortValue">nullable short value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this short? shortValue)
        {
            return shortValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将int值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="intValue">int value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this int intValue)
        {
            return Convert.ToInt64(intValue);
        }

        /// <summary>
        /// <para>将nullable int值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="intValue">nullable int value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this int? intValue, long defaultValue)
        {
            return intValue == null ? defaultValue : Convert.ToInt64(intValue);
        }

        /// <summary>
        /// 将nullable int值转换为长整型值
        /// </summary>
        /// <param name="intValue">nullable int value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this int? intValue)
        {
            return intValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将十进制值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="decimalValue">decimal value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this decimal decimalValue, long defaultValue)
        {
            try
            {
                if (decimalValue > long.MaxValue)
                    decimalValue = defaultValue;

                if (decimalValue < long.MinValue)
                    decimalValue = defaultValue;

                return Convert.ToInt64(decimalValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 将十进制值转换为长整型值
        /// </summary>
        /// <param name="decimalValue">decimal value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this decimal decimalValue)
        {
            return decimalValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将可空的十进制值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="decimalValue">nullable decimal value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this decimal? decimalValue, long defaultValue)
        {
            try
            {
                if (decimalValue == null)
                    decimalValue = defaultValue;

                if (decimalValue > long.MaxValue)
                    decimalValue = defaultValue;

                if (decimalValue < long.MinValue)
                    decimalValue = defaultValue;

                return Convert.ToInt64(decimalValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        ///将可空的十进制值转换为长整型值
        /// </summary>
        /// <param name="decimalValue">nullable decimal value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this decimal? decimalValue)
        {
            return decimalValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将双精度值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="doubleValue">double value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this double doubleValue, long defaultValue)
        {
            try
            {
                if (doubleValue > long.MaxValue)
                    doubleValue = defaultValue;

                if (doubleValue < long.MinValue)
                    doubleValue = defaultValue;

                return Convert.ToInt64(doubleValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 将双精度值转换为长整型值
        /// </summary>
        /// <param name="doubleValue">double value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this double doubleValue)
        {
            return doubleValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将可空值double值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="doubleValue">nullable double value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this double? doubleValue, long defaultValue)
        {
            try
            {
                if (doubleValue == null)
                    doubleValue = defaultValue;

                if (doubleValue > long.MaxValue)
                    doubleValue = defaultValue;

                if (doubleValue < long.MinValue)
                    doubleValue = defaultValue;

                return Convert.ToInt64(doubleValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 将可空值double值转换为长整型值
        /// </summary>
        /// <param name="doubleValue">nullable double value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this double? doubleValue)
        {
            return doubleValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将浮点值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="floatValue">float value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this float floatValue, long defaultValue)
        {
            try
            {
                if (floatValue > long.MaxValue)
                    floatValue = defaultValue;

                if (floatValue < long.MinValue)
                    floatValue = defaultValue;

                return Convert.ToInt64(floatValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        ///将浮点值转换为长整型值
        /// </summary>
        /// <param name="floatValue">float value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this float floatValue)
        {
            return floatValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>将可空的浮点值转换为长整型值</para>
        /// <para>Set default value on invalid convertion</para>
        /// </summary>
        /// <param name="floatValue">nullable float value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this float? floatValue, long defaultValue)
        {
            try
            {
                if (floatValue == null)
                    floatValue = defaultValue;

                if (floatValue > long.MaxValue)
                    floatValue = defaultValue;

                if (floatValue < long.MinValue)
                    floatValue = defaultValue;

                return Convert.ToInt64(floatValue);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 将可空的浮点值转换为长整型值
        /// </summary>
        /// <param name="floatValue">nullable float value</param>
        /// <returns>long value</returns>
        public static long TryParseLong(this float? floatValue)
        {
            return floatValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// <para>尝试解析对象长到长的值</para>
        /// </summary>
        /// <param name="objValue">object to convert</param>
        /// <param name="defaultValue">default return value</param>
        /// <returns>long result</returns>
        public static long TryParseLong(this object objValue, long defaultValue)
        {
            if (objValue == null)
                return defaultValue;

            try
            {
                return objValue.ToString().TryParseLong(defaultValue,
                BasePrimitivesExtensions.GetDefaultLongAllowDefaultConversion(),
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// <para>尝试解析长到长的值</para>
        /// <para>Default value is BasePrimitivesExtensions.GetDefaultlongConversionValue() value</para>
        /// </summary>
        /// <param name="objValue">object to convert</param>
        /// <returns>long result</returns>
        public static long TryParseLong(this object objValue)
        {
            return objValue.TryParseLong(BasePrimitivesExtensions.GetDefaultLongConversionValue());
        }

        /// <summary>
        /// 在长数组中解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <param name="defaultValue">default value when default tryparse</param>
        /// <param name="allowDefaultConversion">Allow default tryparse values</param>
        /// <param name="numberStyle">number style to convert</param>
        /// <param name="culture">culture origin</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue, long[] defaultValue, bool allowDefaultConversion, NumberStyles numberStyle, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(strValue))
                return defaultValue ?? new long[] { };

            var intList = defaultValue != null
                ? defaultValue.ToList()
                : new List<long>();

            foreach (var l in strValue.Split(','))
            {
                var strInt = l ?? "";

                if (String.IsNullOrEmpty(strInt))
                {
                    if (allowDefaultConversion)
                        intList.Add(BasePrimitivesExtensions.GetDefaultLongConversionValue());

                    continue;
                }

                long intConvert;
                if (!long.TryParse(strInt, numberStyle, culture, out intConvert))
                {
                    if (allowDefaultConversion)
                        intList.Add(BasePrimitivesExtensions.GetDefaultLongConversionValue());
                }
                else
                    intList.Add(intConvert);

            }

            return intList.ToArray();
        }

        /// <summary>
        /// 在长数组中解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <param name="defaultValue">default value when default tryparse</param>
        /// <param name="numberStyle">number style to convert</param>
        /// <param name="culture">culture origin</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue, long[] defaultValue, NumberStyles numberStyle, CultureInfo culture)
        {
            return strValue.TryParseLongArray(defaultValue,
                BasePrimitivesExtensions.GetDefaultLongArrayAllowDefaultConversion(),
                numberStyle, culture);
        }

        /// <summary>
        /// 在长数组中解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <param name="numberStyle">number style to convert</param>
        /// <param name="culture">culture origin</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue, NumberStyles numberStyle, CultureInfo culture)
        {
            return strValue.TryParseLongArray(null,
                BasePrimitivesExtensions.GetDefaultLongArrayAllowDefaultConversion(),
                numberStyle, culture);
        }

        /// <summary>
        /// 在长数组中解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <param name="defaultValue">default value when default tryparse</param>
        /// <param name="allowDefaultConversion">Allow default tryparse values</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue, long[] defaultValue, bool allowDefaultConversion)
        {
            return strValue.TryParseLongArray(defaultValue, allowDefaultConversion,
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        ///在长数组中解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <param name="defaultValue">default value when default tryparse</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue, long[] defaultValue)
        {
            return strValue.TryParseLongArray(defaultValue,
                BasePrimitivesExtensions.GetDefaultLongArrayAllowDefaultConversion(),
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        ///短串解析字符串数组
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <returns>long array</returns>
        public static long[] TryParseLongArray(this string strValue)
        {
            return strValue.TryParseLongArray(null,
                BasePrimitivesExtensions.GetDefaultLongArrayAllowDefaultConversion(),
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }

        /// <summary>
        /// 测试字符串值是否为有效的长整型值
        /// </summary>
        /// <param name="strValue">string value</param>
        /// <param name="numberStyle">number style to convert</param>
        /// <param name="culture">culture origin</param>
        /// <returns>true/false</returns>
        public static bool IsValidLong(this string strValue, NumberStyles numberStyle, CultureInfo culture)
        {
            try
            {
                var baseValue = strValue == "1" ? 2 : 1;
                var convertedValue = strValue.TryParseLong(baseValue, true, numberStyle, culture);
                return convertedValue != baseValue;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        ///测试字符串值是否为有效的长整型值
        /// </summary>
        /// <param name="strValue">string value</param>
        /// <returns>true/false</returns>
        public static bool IsValidLong(this string strValue)
        {
            return strValue.IsValidLong(
                BasePrimitivesExtensions.GetDefaultLongNumberStyle(),
                BasePrimitivesExtensions.GetCurrentCulture());
        }
    }
}
