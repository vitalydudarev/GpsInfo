using System;
using System.Text;
using System.Collections.Generic;

namespace GpsInfo
{
    public class GpsInfo
    {
        #region Private Fields

        private byte[] _bytes;
        private IList<DirectoryEntry> _entries;
        private GpsData _gpsData;

        #endregion

        #region Public Constructors

        public GpsInfo(IList<DirectoryEntry> entries, byte[] bytes)
        {
            _bytes = bytes;
            _entries = entries;

            ProcessGpsInfo();
        }

        #endregion

        #region Public Properties

        public GpsData Gps
        {
            get { return _gpsData; }
        }

        #endregion

        #region Private Methods

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
                var bytes = _bytes.GetBytes(entry.ValueOrOffset, entry.Count);
                value = bytes.ToString(false).TrimEnd('\0');
            }

            return value;
        }

        private void ProcessGpsInfo()
        {
            _gpsData = new GpsData();

            foreach (var entry in _entries)
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
                    byte[] readBytes = _bytes.GetBytes(entry.ValueOrOffset, entry.Count * rationalSize);

                    switch (entry.Tag)
                    {
                        case (int)GpsTags.Latitude:
                            _gpsData.Latitude = ParseRational(readBytes);
                            break;
                        case (int)GpsTags.Longitude:
                            _gpsData.Longitude = ParseRational(readBytes);
                            break;
                        case (int)GpsTags.Altitude:
                            _gpsData.Altitude = ParseSimpleRational(readBytes);
                            break;
                        case (int)GpsTags.ImgDirection:
                            _gpsData.ImageDirection = ParseSimpleRational(readBytes);
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

        #endregion

        public class GpsData
        {
            public double Latitude;
            public double Longitude;
            public double Altitude;
            public double ImageDirection;
        }
    }
}

