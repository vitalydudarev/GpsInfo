using System.IO;

namespace GpsInfo
{
    class Program
    {
        private const string FileName = @"C:\Users\Vitalyd\Downloads\WP_20140307_002.jpg";
//        private const string FileName = @"D:\2014-09-17 14.34.54.jpg";

        static void Main(string[] args)
        {
            byte[] bytes = ReadFile(FileName);

            JpegImageInfo jii = new JpegImageInfo(bytes);
            bool isJpeg = jii.IsJpeg();
            bool hasExif = jii.HasExif();
            int exifSize = jii.GetApp1Size();

            byte[] exifData = jii.GetApp1DataTemp();
            ExifInfo exifInfo = new ExifInfo(exifData);
            exifInfo.Parse();
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
