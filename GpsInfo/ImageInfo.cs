using System;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class ImageInfo
    {
        #region Private Fields

        private readonly byte[] _bytes;
        private readonly int _length;
        
        #endregion

        #region Constants
        
        private readonly byte[] SOI = { 0xFF, 0xD8 };   // start of image
        private readonly byte[] EOI = { 0xFF, 0xD9 };   // end of image
        private readonly byte[] APP1 = { 0xFF, 0xE1 };	// EXIF marker
        private const string Exif = "Exif";
        
        #endregion

        #region Public Constructors

        public ImageInfo(byte[] bytes)
        {
            _bytes = bytes;
            _length = _bytes.Length;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the value value that indicates whether the image is of JPEG format.
        /// </summary>
        public bool IsJpeg()
        {
            var firstBytes = _bytes.GetBytes(0, 2);
            var lastBytes = _bytes.GetBytes(_length - 2, 2);

            return firstBytes.SequenceEqual(SOI) && lastBytes.SequenceEqual(EOI);
        }
        
        /// <summary>
        /// Returns the value that indicates whether the image contains EXIF data.
        /// </summary>
        public bool HasExif()
        {
            var bytes = _bytes.GetBytes(6, 4);
            var isExifString = Encoding.UTF8.GetString(bytes).Equals(Exif);
            var hasApp1Marker = HasApp1Marker();

            return hasApp1Marker && isExifString;
        }

        /// <summary>
        /// Returns the byte array of EXIF data of the image.
        /// </summary>
        public byte[] GetExifData()
        {
            var exifDataSize = GetExifSize();
            var bytes = _bytes.GetBytes(4, exifDataSize);

            return bytes;
        }

        #endregion

        #region Private Methods

        private bool HasApp1Marker()
        {
            var bytes = _bytes.GetBytes(2, 2);

            return bytes.SequenceEqual(APP1);
        }

        private int GetExifSize()
        {
            var bytes = _bytes.GetBytes(4, 2);

            return BitConverter.ToUInt16(bytes, 0);
        }

        #endregion
    }
}
