namespace GpsInfo
{
    public class DirectoryEntry
    {
        #region Public Properties

        public ushort Tag
        {
            get { return _tag; }
        }

        public ExifTypes Type
        {
            get { return _type; }
        }

        public int Count
        {
            get { return _count; }
        }

        public int ValueOrOffset
        {
            get { return _valueOrOffset; }
        }

        public string TagName
        {
            get { return _tagName; }
        }

        #endregion

        #region Private Fields

        private readonly ushort _tag;
        private readonly ExifTypes _type;
        private readonly int _count;
        private readonly int _valueOrOffset;
        private readonly string _tagName;

        #endregion

        #region Public Constants

        public const int Size = 12;

        #endregion

        #region Public Constructors

        public DirectoryEntry(byte[] bytes, bool isBigEndian)
        {
            _tag = bytes.GetBytes(0, 2).ToUInt16(isBigEndian);
            _type = (ExifTypes)bytes.GetBytes(2, 2).ToInt16(isBigEndian);
            _count = bytes.GetBytes(4, 4).ToInt32(isBigEndian);
            _valueOrOffset = bytes.GetBytes(8, 4).ToInt32(isBigEndian);
            _tagName = ((TagsEnum.Tags)Tag).ToString();
        }

        #endregion
    }
}
