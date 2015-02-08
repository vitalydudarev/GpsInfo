using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public static class ByteArrayExtensions
    {
        public static byte[] GetBytes(this byte[] bytes, int offset, int count)
        {
            byte[] result = new byte[count];
            int j = 0;

            for (int i = offset; i < offset + count; i++)
            {
                result[j] = bytes[i];
                j++;
            }

            return result;
        }

        public static short ToInt16(this byte[] bytes, bool bigEndian = true)
        {
            if (bigEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ToUInt16(this byte[] bytes, bool bigEndian = true)
        {
            if (bigEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static int ToInt32(this byte[] bytes, bool bigEndian = true)
        {
            if (bigEndian)
                Array.Reverse(bytes);
            
            return BitConverter.ToInt32(bytes, 0);
        }

        public static int ToUInt32(this byte[] bytes, bool bigEndian = true)
        {
            if (bigEndian)
                Array.Reverse(bytes);

            var value = BitConverter.ToUInt32(bytes, 0);;

            return Convert.ToInt32(value);
        }

        public static string ToString(this byte[] bytes, bool bigEndian = true)
        {
            if (bigEndian)
                Array.Reverse(bytes);

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
