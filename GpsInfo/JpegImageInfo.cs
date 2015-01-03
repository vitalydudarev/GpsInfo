using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class JpegImageInfo
    {
        #region Private fields
        private readonly byte[] _byteArray;
        private readonly ArrayHelper _arrayHelper;
        #endregion

        #region JPEG markers
        private readonly byte[] SOI = { 0xFF, 0xD8 };		// start of image
        private readonly byte[] EOI = { 0xFF, 0xD9 };		// end of image
        private readonly byte[] APP1 = { 0xFF, 0xE1 };	// APP1 is an EXIF marker
        #endregion

        public JpegImageInfo(byte[] byteArray)
        {
            _byteArray = byteArray;
            _arrayHelper = new ArrayHelper(byteArray);
        }

        public bool IsJpeg()
        {
            byte[] firstBytes = _arrayHelper.GetBytes(0, 2);
            byte[] lastBytes = _arrayHelper.GetBytes(_byteArray.Length - 2, 2);

            return _arrayHelper.Equals(firstBytes, SOI) && _arrayHelper.Equals(lastBytes, EOI);
        }

        public int GetApp1Size()
        {
            byte[] bytes = _arrayHelper.GetBytes(4, 2, false);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public byte[] GetApp1DataTemp()
        {
//            int exifDataSize = GetApp1Size();
            byte[] bytes = _byteArray.GetBytes(4, _byteArray.Length - 4);

            return bytes;
        }

        public byte[] GetApp1Data()
        {
            int exifDataSize = GetApp1Size();
            byte[] bytes = _arrayHelper.GetBytes(4, exifDataSize);

            return bytes;
        }

        public bool HasExif()
        {
            byte[] bytes = _arrayHelper.GetBytes(2, 2);

            return BitConverter.ToString(bytes).Equals("Exif");
        }
    }
}
