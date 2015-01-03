using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class TagsEnum
    {
        public enum Tags
        {
            ExifIfd = 0x8769,
            GpsIfd = 0x8825,
            InteroperabilityIfd = 0xA005,

            ImageWidth = 0x100,
            ImageLength = 0x101,
            BitsPerSample = 0x102,
            Compression = 0x103,
            PhotometricInterpretation = 0x106,
            ImageDesription = 0x10e,
            Make = 0x10f,
            Model = 0x110,
            StripOffsets = 0x111,
            Orientation = 0x112,
            SamplesPerPixel = 0x115,
            RowsPerStrip = 0x116,
            StripByteCounts = 0x117,
            XResolution = 0x11a,
            YResolution = 0x11b,
            PlanarConfiguration = 0x11c,
            ResolutionUnit = 0x128,
            TransferFunction = 0x12d,
            Software = 0x131,
            DateTime = 0x132,
            Artist = 0x13b,
            WhitePoint = 0x13e,
            PrimaryChromaticities = 0x13f,
            YCbCrCoefficients = 0x211,
            YcbCrSubSampling = 0x212,
            YcbCrPositioning = 0x213,
            ReferenceBlackWhite = 0x214,
            Copyrigth = 0x8298
        }
    }
}
