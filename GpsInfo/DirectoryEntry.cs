namespace GpsInfo
{
    public class DirectoryEntry : ITiffElement
    {
        #region Public Properties

        public ushort Tag { get; private set; }

        public ExifTypes Type { get; private set; }

        public int Count { get; private set; }

        public int ValueOrOffset { get; private set; }

        public string TagName { get; private set; }

        #endregion

        #region Private Fields

        private readonly byte[] _bytes;
        private readonly bool _isBigEndian;

        #endregion

        #region Public Constants

        public const int Size = 12;

        #endregion

        #region Public Constructors

        public DirectoryEntry(byte[] bytes, bool isBigEndian)
        {
            _bytes = bytes;
            _isBigEndian = isBigEndian;
        }

        #endregion

        #region Public Methods

        public ITiffElement Init()
        {
            Tag = _bytes.GetBytes(0, 2).ToUInt16(_isBigEndian);
            Type = (ExifTypes)_bytes.GetBytes(2, 2).ToInt16(_isBigEndian);
            Count = _bytes.GetBytes(4, 4).ToInt32(_isBigEndian);
            ValueOrOffset = _bytes.GetBytes(8, 4).ToInt32(_isBigEndian);
            TagName = ((ExifTags)Tag).ToString();

            return this;
        }

        #endregion
    }
}
