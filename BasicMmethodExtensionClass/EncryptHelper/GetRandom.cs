﻿using System;
using System.Globalization;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// 随机util类
    /// </summary>
    public static class GetRandom
    {
        /// <summary>
        /// 使用字母，数字和符号创建随机散列
        /// </summary>
        /// <param name="size">hash size</param>
        /// <returns>randon hash</returns>
        public static string CreateRandonHash(int size)
        {
            try
            {
                if (size <= 0)
                    return "";

                const string letters = "ABCDEFGHIJKLMNOPQRSTWXYZ";

                const string numbers = "0123456789";

                const string symbols = "~!@#$%^&*()_-+=[{]}|><,.?/";

                var hash = string.Empty;
                var rand = new Random();

                for (var cont = 0; cont < size; cont++)
                {
                    switch (rand.Next(0, 3))
                    {
                        case 1:
                            hash = hash + numbers[rand.Next(0, 10)];
                            break;
                        case 2:
                            hash = hash + symbols[rand.Next(0, 26)];
                            break;
                        default:
                            hash = hash + ((cont % 3 == 0)
                                ? letters[rand.Next(0, 24)].ToString(CultureInfo.InvariantCulture)
                                : (letters[rand.Next(0, 24)]).ToString(CultureInfo.InvariantCulture).ToLower());
                            break;
                    }
                }

                return hash;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
