using Newtonsoft.Json.Linq;

namespace BasicMmethodExtensionClass.JsonOperationHelper
{
    /// <summary>
    /// JSON帮助类
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// 将JArray转换为整型数组
        /// </summary>
        /// <param name="ja">JArray对象</param>
        /// <returns>整型数组</returns>
        public static int[] IntArrayFromJArray(JArray ja)
        {
            if (ja == null || ja.Count == 0)
            {
                return new int[0];
            }
            int length = ja.Count;
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ja[i].Value<int>();
            }
            return array;
        }

        /// <summary>
        /// 将JArray转换为字符串数组
        /// </summary>
        /// <param name="ja">JArray对象</param>
        /// <returns>字符串数组</returns>
        public static string[] StringArrayFromJArray(JArray ja)
        {
            if (ja == null || ja.Count == 0)
            {
                return new string[0];
            }
            int length = ja.Count;

            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ja[i].Value<string>();
            }
            return array;
        }

        /// <summary>
        /// 将JArray转换为对象数组
        /// </summary>
        /// <param name="ja">JArray对象</param>
        /// <returns>对象数组</returns>
        public static object[] ObjectArrayFromJArray(JArray ja)
        {
            if (ja == null || ja.Count == 0)
            {
                return new object[0];
            }

            int length = ja.Count;

            object[] array = new object[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ja[i].Value<JValue>().Value;
            }
            return array;
        }

    }
}
