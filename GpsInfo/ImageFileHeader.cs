namespace GpsInfo
{
    public class ImageFileHeader
    {
        public string ByteOrder;
        public short Number42;
        public int FirstIfdOffset;

        public ImageFileHeader(byte[] bytes)
        {
            ByteOrder = bytes.GetBytes(0, sizeof(short)).ToString(true);
            Number42 = bytes.GetBytes(2, sizeof(short)).ToInt16(ByteOrder == "MM");
            FirstIfdOffset = bytes.GetBytes(4, sizeof(int)).ToInt32(ByteOrder == "MM");
        }
    }
}
