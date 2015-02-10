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

        #region Constants

        /// <summary>
        /// Motorola byte align.
        /// </summary>
        private const string MM = "MM";

        #endregion

        #region Public Constructors

        public ImageFileHeader(byte[] bytes)
        {
            _bytes = bytes;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            ByteOrder = _bytes.GetBytes(0, 2).ToString(true);
            var isBigEndian = ByteOrder == MM;
            Number42 = _bytes.GetBytes(2, 2).ToInt16(isBigEndian);
            FirstIfdOffset = _bytes.GetBytes(4, 4).ToUInt32(isBigEndian);
        }

        #endregion
    }
}
