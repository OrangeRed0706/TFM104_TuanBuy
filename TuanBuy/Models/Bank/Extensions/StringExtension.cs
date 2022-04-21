using System;
using System.Collections.Generic;
using System.Linq;

namespace TuanBuy.Models.Bank.Extensions
{
    /// <summary>
    /// 字串擴充功能
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string source)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var outputLength = source.Length / 2;
                var output = new byte[outputLength];

                for (var i = 0; i < outputLength; i++)
                {
                    output[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
                }

                result = output;
            }

            return result;
        }

        public static string GetValueByKey(this IEnumerable<KeyValuePair<string, string>> data, string text)
        {
            return data.FirstOrDefault(x => x.Key.ToLower().Contains(text)).Value;
        }
    }
}