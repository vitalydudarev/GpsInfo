namespace GpsInfo
{
    public abstract class TiffElement
    {
        #region Protected Fields

        protected byte[] _bytes;
        protected bool _isBigEndian;

        #endregion

        #region Protected Constructors

        protected TiffElement(byte[] bytes, bool isBigEndian)
        {
            _bytes = bytes;
            _isBigEndian = isBigEndian;
        }

        #endregion

        #region Public Virtual Methods

        public virtual void Init()
        {
        }

        #endregion
    }
}
