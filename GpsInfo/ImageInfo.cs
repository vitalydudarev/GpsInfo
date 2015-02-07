using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class ImageInfo
    {
        #region Private Fields

        private readonly byte[] _bytes;
        private readonly int _length;
        private readonly Dictionary<JpegMarkers, byte[]> _markers;

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
            _markers = new Dictionary<JpegMarkers, byte[]>();

            InitMarkers();
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
            byte[] bodyBytes;

            if (_markers.TryGetValue(JpegMarkers.APP1, out bodyBytes))
            {
                var bytes = bodyBytes.GetBytes(2, 4);
                var isExifString = Encoding.UTF8.GetString(bytes).Equals(Exif);

                return isExifString;
            }

            return false;
        }

        /// <summary>
        /// Returns the byte array of EXIF data of the image.
        /// </summary>
        public byte[] GetExifData()
        {
            return _markers[JpegMarkers.APP1];
        }

        #endregion

        #region Private Methods

        private void InitMarkers()
        {
            int offset = 0;

            for (int i = 0; i < 2; i++)
            {
                var marker = GetMarker(2 + offset);
                var length = GetMarkerLength(4 + offset);
                var bytes = _bytes.GetBytes(4 + offset, length);

                offset += length + 2;

                _markers.Add(marker, bytes);
            }
        }

        private JpegMarkers GetMarker(int offset)
        {
            var bytes = _bytes.GetBytes(offset, 2);
            Array.Reverse(bytes);
            var value = BitConverter.ToUInt16(bytes, 0);

            return (JpegMarkers) value;
        }

        private int GetMarkerLength(int offset)
        {
            var bytes = _bytes.GetBytes(offset, 2);
            Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }

        #endregion
    }
}
