using System;

namespace GpsInfo
{
    public class ImageFileHeader : TiffElement
    {
        #region Public Properties

        public string ByteOrder { get; private set; }

        public short Number42 { get; private set; }

        public int FirstIfdOffset { get; private set; }

        #endregion

        #region Private Fields

        private readonly Func<string, bool> _isBigEndianFunc; 

        #endregion
        
        #region Public Constructors

        public ImageFileHeader(byte[] bytes, Func<string, bool> isBigEndianFunc) : base(bytes, false)
        {
            _isBigEndianFunc = isBigEndianFunc;
        }

        #endregion

        #region Public Methods

        public override void Init()
        {
            ByteOrder = _bytes.GetBytes(0, 2).ToString(false);
            _isBigEndian = _isBigEndianFunc(ByteOrder);
            Number42 = _bytes.GetBytes(2, 2).ToInt16(_isBigEndian);
            FirstIfdOffset = _bytes.GetBytes(4, 4).ToUInt32(_isBigEndian);
        }

        #endregion
    }
}
