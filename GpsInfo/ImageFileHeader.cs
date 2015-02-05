namespace GpsInfo
{
    public class ImageFileHeader : ITiffElement
    {
        #region Public Properties

        public string ByteOrder { get; private set; }

        public short Number42 { get; private set; }

        public int FirstIfdOffset { get; private set; }

        #endregion

        #region Private Fields

        private readonly byte[] _bytes;

        #endregion

        #region Public Constructors

        public ImageFileHeader(byte[] bytes)
        {
            _bytes = bytes;
        }

        #endregion

        #region Public Methods

        public ITiffElement Init()
        {
            ByteOrder = _bytes.GetBytes(0, sizeof(short)).ToString(true);
            Number42 = _bytes.GetBytes(2, sizeof(short)).ToInt16(ByteOrder == "MM");
            FirstIfdOffset = _bytes.GetBytes(4, sizeof(int)).ToInt32(ByteOrder == "MM");

            return this;
        }

        #endregion
    }
}
