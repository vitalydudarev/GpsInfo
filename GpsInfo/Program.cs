using System.IO;

namespace GpsInfo
{
    class Program
    {
        private const string FileName = @"..\..\data\2014-09-17 14.34.54.jpg";

        static void Main(string[] args)
        {
            var bytes = ReadFile(FileName);

            var imageInfo = new ImageInfo(bytes);
            var isJpeg = imageInfo.IsJpeg();
            var hasExif = imageInfo.HasExif;

            if (isJpeg && hasExif)
            {
                var exifData = imageInfo.GetExifData();
                var exifInfo = new ExifInfo(exifData);
                exifInfo.Parse();
            }
        }

        private static byte[] ReadFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            int streamLength = (int)fs.Length;
            byte[] buffer = new byte[streamLength];
            fs.Read(buffer, 0, streamLength);

            return buffer;
        }
    }
}
