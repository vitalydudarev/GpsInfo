using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class ExifInfo
    {
        #region Private fields

        private readonly byte[] _tiffData;
        private readonly int _length;
        private bool _isBigEndian;
        private readonly Dictionary<ushort, IFD> _ifds; 

        #endregion

        #region Constants

        private const string MM = "MM";

        #endregion

        #region Public Constructors

        public ExifInfo(byte[] bytes)
        {
            _tiffData = bytes.GetBytes(10, bytes.Length - 10);
            _length = _tiffData.Length;
            _ifds = new Dictionary<ushort, IFD>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value that indicates whether the image contains GPS data.
        /// </summary>
        public bool HasGps
        {
            get { return _ifds.ContainsKey((ushort)ExifTags.GpsIfd); }
        }

        public byte[] TiffData
        {
            get { return _tiffData; }
        }

        #endregion

        #region Public Methods

        public void Parse()
        {
            var header = ParseTiffHeader();
            var firstIfdOffset = header.FirstIfdOffset;

            _isBigEndian = IsBigEndian(header.ByteOrder);

            ParseIfds(firstIfdOffset);
        }

        public IList<DirectoryEntry> GetGpsData()
        {
            return _ifds[(ushort)ExifTags.GpsIfd].Entries;
        }

        #endregion

        private IFD GetIfd(int offset)
        {
            var bytes = _tiffData.GetBytes(offset, (_length - (8 + offset)));
            var ifd = new IFD(bytes, _isBigEndian);
            ifd.Init();

            return ifd;
        }

        private void ParseIfds(int firstIfdOffset)
        {
            var ifd = GetIfd(firstIfdOffset);
            var entries = ifd.Entries;

            _ifds.Add(0, ifd);

            int i = 1;

            while (ifd.OffsetOfNextIfd != 0 && ifd.NumberOfDirectoryEntries > 0)
            {
                ifd = GetIfd(ifd.OffsetOfNextIfd);
                _ifds.Add((ushort)i, ifd);

                i++;
            }

            var additionalIfds = new [] { ExifTags.ExifIfd, ExifTags.GpsIfd };

            foreach (var additionalIfd in additionalIfds)
            {
                var entry = entries.Select(a => a).FirstOrDefault(a => a.Tag == (ushort)additionalIfd);
                if (entry != null)
                {
                    ifd = GetIfd(entry.ValueOrOffset);
                    _ifds.Add(entry.Tag, ifd);
                }
            }
        }

        // TIFF header: 
        // Byte Order (2 bytes)
        // '42' (2 bytes)
        // Offset of IFD (4 bytes)
        private ImageFileHeader ParseTiffHeader()
        {
            var headerBytes = _tiffData.GetBytes(0, 8);
            var header = new ImageFileHeader(headerBytes, IsBigEndian);
            header.Init();

            return header;
        }

        private static bool IsBigEndian(string str)
        {
            return str == MM;
        }

        public interface IParser
        {
            object Parse(byte[] bytes);
        }

        public class RationalParser : IParser
        {
            public object Parse(byte[] bytes)
            {
                return bytes.GetBytes(0, 4).ToInt32() / bytes.GetBytes(4, 4).ToInt32();
            }
        }
    }
}
