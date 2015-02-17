using System;
using System.Text;

namespace GpsInfo
{
    public static class ByteArrayExtensions
    {
        #region Public Methods

        public static byte[] GetBytes(this byte[] bytes, int offset, int count)
        {
            var result = new byte[count];
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
            PrepareArray(bytes, bigEndian);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ToUInt16(this byte[] bytes, bool bigEndian = true)
        {
            PrepareArray(bytes, bigEndian);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static int ToInt32(this byte[] bytes, bool bigEndian = true)
        {
            PrepareArray(bytes, bigEndian);
            
            return BitConverter.ToInt32(bytes, 0);
        }

        public static int ToUInt32(this byte[] bytes, bool bigEndian = true)
        {
            PrepareArray(bytes, bigEndian);

            var value = BitConverter.ToUInt32(bytes, 0);;

            return Convert.ToInt32(value);
        }

        public static string ToString(this byte[] bytes, bool bigEndian = true)
        {
            PrepareArray(bytes, bigEndian);

            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[] TrimZeros(this byte[] bytes)
        {
            var startIndex = Array.FindIndex(bytes, a => a != 0);
            var endIndex = Array.FindLastIndex(bytes, a => a != 0);
            var length = endIndex - startIndex + 1;

            var newArray = new byte[length];
            Array.Copy(bytes, startIndex, newArray, 0, length);

            return newArray;
        }

        #endregion

        #region Private Methods

        private static void PrepareArray(byte[] bytes, bool isBigEndian)
        {
            if (isBigEndian)
                Array.Reverse(bytes);
        }

        #endregion
    }
}
