namespace GpsInfo
{
    public class IFD
    {
        #region Public Properties

        public short NumberOfDirectoryEntries
        {
            get { return _numberOfDirectoryEntries; }
        }

        public int OffsetOfNextIfd
        {
            get { return _offsetOfNextIfd; }
        }

        public DirectoryEntry[] Entries
        {
            get { return _entries; }
        }

        #endregion

        #region Private Fields

        private readonly short _numberOfDirectoryEntries;
        private readonly int _offsetOfNextIfd;
        private readonly DirectoryEntry[] _entries;

        #endregion

        #region Public Constructors

        public IFD(byte[] bytes, bool isBigEndian)
        {
            _numberOfDirectoryEntries = bytes.GetBytes(0, 2).ToInt16(isBigEndian);
            var entriesBytes = bytes.GetBytes(2, _numberOfDirectoryEntries * DirectoryEntry.Size);
            _offsetOfNextIfd = bytes.GetBytes(2 + entriesBytes.Length, 4).ToInt32(isBigEndian);

            _entries = new DirectoryEntry[_numberOfDirectoryEntries];
            int count = 0;

            for (int i = 0; i < entriesBytes.Length; i = i + DirectoryEntry.Size)
            {
                var entry = new DirectoryEntry(entriesBytes.GetBytes(i, DirectoryEntry.Size), isBigEndian);
                _entries[count] = entry;
                count++;
            }
        }

        #endregion
    }
}
