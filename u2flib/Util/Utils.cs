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

        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}