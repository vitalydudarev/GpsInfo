namespace GpsInfo
{
    public class DirectoryEntry
    {
        public ushort Tag;
        public ExifTypes Type;
        public int Count;
        public int ValueOrOffset;
        public string TagName;

        public const int Size = 12;

        public DirectoryEntry(byte[] bytes, bool isBigEndian)
        {
            Tag = bytes.GetBytes(0, 2).ToUInt16(isBigEndian);
            Type = (ExifTypes)bytes.GetBytes(2, 2).ToInt16(isBigEndian);
            Count = bytes.GetBytes(4, 4).ToInt32(isBigEndian);
            ValueOrOffset = bytes.GetBytes(8, 4).ToInt32(isBigEndian);
            TagName = ((TagsEnum.Tags)Tag).ToString();
        }
    }
}
