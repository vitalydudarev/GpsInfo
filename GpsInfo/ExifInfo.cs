using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GpsInfo
{
    public class ExifInfo
    {
        #region Constants
        private const int App1BodyOffset = 8;
        private const int TiffBodyOffset = 8;
        #endregion

        #region Private fields
        private byte[] _byteArray;
        private byte[] _app1Body;
        private readonly byte[] _tiffBody;
        #endregion

        /// <summary>
        /// The constructor of ExifInfo class. Takes the body of JPEG image (byte array between SOI and EOI).
        /// Takes byte array starting from 4th byte of an image (APP1 size).
        /// </summary>
        /// <param name="byteArray">Byte array.</param>
        public ExifInfo(byte[] byteArray)
        {
            _byteArray = byteArray;
            _tiffBody = _byteArray.GetBytes(TiffBodyOffset, _byteArray.Length - TiffBodyOffset);
        }

        public void Parse()
        {
            var header = ParseHeader();
            ParseIfds(header);
        }

        private void ParseIfds(ImageFileHeader header)
        {
            bool isBigEndian = header.ByteOrder == "MM";
            var ifds = new List<IFD>();
            var allEntries = new List<DirectoryEntry>();
            var tiff = _tiffBody.GetBytes(header.FirstIfdOffset, (_tiffBody.Length - (TiffBodyOffset + header.FirstIfdOffset)));
            var firstIfd = new IFD(tiff, isBigEndian);
            ifds.Add(firstIfd);
            var entries = firstIfd.Entries;
            allEntries.AddRange(entries);

            var ifd = firstIfd;

            while (ifd.OffsetOfNextIfd != 0 && ifd.NumberOfDirectoryEntries > 0)
            {
                var bytes = _tiffBody.GetBytes(ifd.OffsetOfNextIfd, (_tiffBody.Length - (TiffBodyOffset + ifd.OffsetOfNextIfd)));
                var newIfd = new IFD(bytes, isBigEndian);
                ifds.Add(newIfd);
                allEntries.AddRange(newIfd.Entries);

                ifd = newIfd;
            }

            var exifEntry = allEntries.Select(a => a).FirstOrDefault(a => a.Tag == (ushort)TagsEnum.Tags.ExifIfd);
            if (exifEntry != null)
            {
                var bytes = _tiffBody.GetBytes(exifEntry.ValueOrOffset, (_tiffBody.Length - (TiffBodyOffset + exifEntry.ValueOrOffset)));
                var exifIfd = new IFD(bytes, isBigEndian);
                ifds.Add(exifIfd);
                allEntries.AddRange(exifIfd.Entries);
            }

            var gpsEntry = allEntries.Select(a => a).FirstOrDefault(a => a.Tag == (ushort)TagsEnum.Tags.GpsIfd);
            if (gpsEntry != null)
            {
                var bytes = _tiffBody.GetBytes(gpsEntry.ValueOrOffset, (_tiffBody.Length - (TiffBodyOffset + gpsEntry.ValueOrOffset)));
                var gpsIfd = new IFD(bytes, isBigEndian);

                ProcessGpsInfo(gpsIfd.Entries, _tiffBody);
            }
            
            foreach (var entry in allEntries)
            {
                if (entry.Type != ExifTypes.ASCII)
                    continue;

                string value = GetStringValue(entry);
                Console.WriteLine(entry.Tag + " " + value);
            }
        }

        private string GetStringValue(DirectoryEntry entry)
        {
            string value;

            // if Count is less or equals to 4, the value is stored in ValueOrOffset field of Directory Entry
            if (entry.Count <= 4)
            {
                var bytes = BitConverter.GetBytes(entry.ValueOrOffset);
                value = Encoding.ASCII.GetString(bytes).TrimStart(new []{'\0'});
            }
            else
            {
                var bytes = _tiffBody.GetBytes(entry.ValueOrOffset, entry.Count);
                value = bytes.ToString(false).TrimEnd(new []{ '\0' });
            }

            return value;
        }

        private void ProcessGpsInfo(IEnumerable<DirectoryEntry> entries, byte[] bytes)
        {
            var gps = new Gps();

            foreach (var entry in entries)
            {
                if (entry.Type == ExifTypes.ASCII)
                {
                    switch (entry.Tag)
                    {
                        case (int)GpsTags.LatitudeRef:
                        case (int)GpsTags.LongitudeRef:
                        case (int)GpsTags.ImgDirectionRef:
                        {
                            string value = GetStringValue(entry);
                        }
                            break;
                    }
                }

                if (entry.Type == ExifTypes.RATIONAL)
                {
                    const int rationalSize = 8;
                    byte[] readBytes = bytes.GetBytes(entry.ValueOrOffset, entry.Count * rationalSize);

                    switch (entry.Tag)
                    {
                        case (int)GpsTags.Latitude:
                            gps.Latitude = ParseRational(readBytes);
                            break;
                        case (int)GpsTags.Longitude:
                            gps.Longitude = ParseRational(readBytes);
                            break;
                        case (int)GpsTags.Altitude:
                            gps.Altitude = ParseSimpleRational(readBytes);
                            break;
                        case (int)GpsTags.ImgDirection:
                            gps.ImageDirection = ParseSimpleRational(readBytes);
                            break;
                    }
                }
            }
        }

        private static double ParseSimpleRational(byte[] bytes)
        {
            return bytes.GetBytes(0, 4).ToInt32() / bytes.GetBytes(4, 4).ToInt32();
        }

        private static double ParseRational(byte[] bytes)
        {
            byte[] degreesBytes = bytes.GetBytes(0, 8);
            byte[] minutesBytes = bytes.GetBytes(8, 8);
            byte[] secondsBytes = bytes.GetBytes(16, 8);

            double degrees = ParseSimpleRational(degreesBytes);
            double minutes = ParseSimpleRational(minutesBytes);
            double seconds = ParseSimpleRational(secondsBytes);

            return degrees + minutes / 60 + seconds / 3600;
        }

        private ImageFileHeader ParseHeader()
        {
            var headerBytes = _tiffBody.GetBytes(0, 8);
            var header = new ImageFileHeader(headerBytes);

            return header;
        }

        public class Gps
        {
            public double Latitude;
            public double Longitude;
            public double Altitude;
            public double ImageDirection;
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
