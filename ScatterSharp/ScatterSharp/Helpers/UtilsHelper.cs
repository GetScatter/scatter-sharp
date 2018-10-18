using Cryptography.ECDSA;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ScatterSharp.Helpers
{
    public class UtilsHelper
    {
        public static string RandomNumber(int size = 6)
        {
            return ByteArrayToHexString(RandomNumberBytes(size));
        }

        public static byte[] RandomNumberBytes(int size = 6)
        {
            var r = RandomNumberGenerator.Create();
            byte[] numberBytes = new byte[size];
            r.GetBytes(numberBytes);
            return numberBytes;
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

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
