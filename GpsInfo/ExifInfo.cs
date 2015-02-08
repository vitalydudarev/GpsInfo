﻿using System;
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

        #endregion

        public ExifInfo(byte[] bytes)
        {
            _tiffData = bytes.GetBytes(8, bytes.Length - 8);
            _length = _tiffData.Length;
        }

        public void Parse()
        {
            var header = ParseTiffHeader();
            var isBigEndian = header.ByteOrder == "MM";
            var firstIfdOffset = header.FirstIfdOffset;

            ParseIfds(isBigEndian, firstIfdOffset);
        }

        private void ParseIfds(bool isBigEndian, int firstIfdOffset)
        {
            var ifds = new List<IFD>();
            var allEntries = new List<DirectoryEntry>();
            var tiff = _tiffData.GetBytes(firstIfdOffset, (_length - (8 + firstIfdOffset)));
            var firstIfd = (IFD)new IFD(tiff, isBigEndian).Init();
            ifds.Add(firstIfd);
            var entries = firstIfd.Entries;
            allEntries.AddRange(entries);

            var ifd = firstIfd;

            while (ifd.OffsetOfNextIfd != 0 && ifd.NumberOfDirectoryEntries > 0)
            {
                var bytes = _tiffData.GetBytes(ifd.OffsetOfNextIfd, (_length - (8 + ifd.OffsetOfNextIfd)));
                var newIfd = (IFD)new IFD(bytes, isBigEndian).Init();
                ifds.Add(newIfd);
                allEntries.AddRange(newIfd.Entries);

                ifd = newIfd;
            }

            var exifEntry = allEntries.Select(a => a).FirstOrDefault(a => a.Tag == (ushort)ExifTags.ExifIfd);
            if (exifEntry != null)
            {
                var bytes = _tiffData.GetBytes(exifEntry.ValueOrOffset, (_length - (8 + exifEntry.ValueOrOffset)));
                var exifIfd = (IFD)new IFD(bytes, isBigEndian).Init();
                ifds.Add(exifIfd);
                allEntries.AddRange(exifIfd.Entries);
            }

            var gpsEntry = allEntries.Select(a => a).FirstOrDefault(a => a.Tag == (ushort)ExifTags.GpsIfd);
            if (gpsEntry != null)
            {
                var bytes = _tiffData.GetBytes(gpsEntry.ValueOrOffset, (_length - (8 + gpsEntry.ValueOrOffset)));
                var gpsIfd = (IFD)new IFD(bytes, isBigEndian).Init();

                ProcessGpsInfo(gpsIfd.Entries, _tiffData);
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
                value = Encoding.ASCII.GetString(bytes).TrimStart('\0');
            }
            else
            {
                var bytes = _tiffData.GetBytes(entry.ValueOrOffset, entry.Count);
                value = bytes.ToString(false).TrimEnd('\0');
            }

            return value;
        }

        private void ProcessGpsInfo(IEnumerable<DirectoryEntry> entries, byte[] bytes)
        {
            var gps = new ExifInfo.Gps();

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

        private ImageFileHeader ParseTiffHeader()
        {
            var headerBytes = _tiffData.GetBytes(0, 8);
            var header = new ImageFileHeader(headerBytes);
            header.Init();

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

        public class RationalParser : ExifInfo.IParser
        {
            public object Parse(byte[] bytes)
            {
                return bytes.GetBytes(0, 4).ToInt32() / bytes.GetBytes(4, 4).ToInt32();
            }
        }
    }
}
