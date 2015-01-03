using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class ArrayHelper
    {
        private byte[] _byteArray;

        public ArrayHelper(byte[] byteArray)
        {
            _byteArray = byteArray;
        }

        public byte[] GetBytes(int offset, int count, bool bigEndian = false)
        {
            byte[] result = new byte[count];
            int j = 0;

            for (int i = offset; i < offset + count; i++)
            {
                result[j] = _byteArray[i];
                j++;
            }

            if (bigEndian)
                Array.Reverse(result);

            return result;
        }

        public bool Equals(byte[] arrayOne, byte[] arrayTwo)
        {
            return Enumerable.SequenceEqual(arrayOne, arrayTwo);
        }
    }
}
