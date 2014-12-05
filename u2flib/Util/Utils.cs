using System.IO;

namespace u2flib.Util
{
    public class Utils
    {
        /// <summary>
        /// Formats the string to base64. 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>properly base64 string</returns>
        public static string FormatStringToBase64(string input)
        {
            input = input.Replace('-', '+');
            input = input.Replace('_', '/');
            int mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                input += new string('=', 4 - mod4);
            }

            return input;
        }

        public static byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    memoryStream.Write(buffer, 0, count);

                return memoryStream.ToArray();
            }
        }
    }
}