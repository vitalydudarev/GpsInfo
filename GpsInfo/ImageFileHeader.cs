namespace GpsInfo
{
    public class ImageFileHeader
    {
        #region Public Properties

        public string ByteOrder
        {
            get { return _byteOrder; }
        }

        public short Number42
        {
            get { return _number42; }
        }

        public int FirstIfdOffset
        {
            get { return _firstIfdOffset; }
        }

        #endregion

        #region Private Fields

        private readonly string _byteOrder;
        private readonly short _number42;
        private readonly int _firstIfdOffset;

        #endregion

        #region Public Constructors

        public ImageFileHeader(byte[] bytes)
        {
            _byteOrder = bytes.GetBytes(0, sizeof(short)).ToString(true);
            _number42 = bytes.GetBytes(2, sizeof(short)).ToInt16(ByteOrder == "MM");
            _firstIfdOffset = bytes.GetBytes(4, sizeof(int)).ToInt32(ByteOrder == "MM");
        }

        #endregion
    }
}
