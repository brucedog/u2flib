using System;
using System.Text;

namespace u2flib.Util
{
    public static class Utils
    {
        /// <summary>
        /// Converts byte array to a properly formatted base64 string
        /// </summary>
        /// <param name="input">The argument.</param>
        /// <returns></returns>
        public static string ByteArrayToBase64String(byte[] input)
        {
            string result = Convert.ToBase64String(input);
            result = result.TrimEnd('='); 
            result = result.Replace('+', '-'); 
            result = result.Replace('/', '_'); 

            return result;
        }

        /// <summary>
        /// Formats string to proper base64 string and returns it as a byte array.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static byte[] Base64StringToByteArray(string input)
        {
            input = input.Replace('-', '+'); 
            input = input.Replace('_', '/'); 

            int mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                input += new string('=', 4 - mod4);
            }

            return Convert.FromBase64String(input); 
        }

        /// <summary>
        /// Convert string into UTF8 byte[]
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns>UTF8 encoded byte[]</returns>
        public static byte[] GetBytes(string stringToConvert)
        {
            return Encoding.UTF8.GetBytes(stringToConvert);
        }

        /// <summary>
        /// Converts byte[] to UTF8 encoded string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>UTF8 encoded string</returns>
        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}