using System;
using System.Security.Cryptography;
using System.Text;

namespace ScatterSharp.Core.Helpers
{
    public class UtilsHelper
    {
        /// <summary>
        /// Generate hex encoded random number
        /// </summary>
        /// <param name="size">number size in bytes</param>
        /// <returns></returns>
        public static string RandomNumber(int size = 6)
        {
            return ByteArrayToHexString(RandomNumberBytes(size));
        }

        /// <summary>
        /// Generate random number and convert to byte array
        /// </summary>
        /// <param name="size">number size in bytes</param>
        /// <returns></returns>
        public static byte[] RandomNumberBytes(int size = 6)
        {
            var r = RandomNumberGenerator.Create();
            byte[] numberBytes = new byte[size];
            r.GetBytes(numberBytes);
            return numberBytes;
        }

        /// <summary>
        /// Encode byte array to hexadecimal string
        /// </summary>
        /// <param name="ba">byte array to convert</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

        /// <summary>
        /// Decode hexadecimal string to byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            var l = hex.Length / 2;
            var result = new byte[l];
            for (var i = 0; i < l; ++i)
                result[i] = (byte)Convert.ToInt32(hex.Substring(i * 2, 2), 16);
            return result;
        }
    }
}
