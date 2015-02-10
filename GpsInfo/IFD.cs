namespace GpsInfo
{
    public class IFD : TiffElement
    {
        #region Public Properties

        public short NumberOfDirectoryEntries { get; private set; }

        public int OffsetOfNextIfd { get; private set; }

        public DirectoryEntry[] Entries { get; private set; }

        #endregion
        
        #region Public Constructors

        public IFD(byte[] bytes, bool isBigEndian) : base(bytes, isBigEndian)
        {
        }

        #endregion

        public override void Init()
        {
            NumberOfDirectoryEntries = _bytes.GetBytes(0, 2).ToInt16(_isBigEndian);
            var entriesBytes = _bytes.GetBytes(2, NumberOfDirectoryEntries * DirectoryEntry.Size);
            OffsetOfNextIfd = _bytes.GetBytes(2 + entriesBytes.Length, 4).ToInt32(_isBigEndian);

            Entries = new DirectoryEntry[NumberOfDirectoryEntries];
            int count = 0;

            for (int i = 0; i < entriesBytes.Length; i = i + DirectoryEntry.Size)
            {
                var entry = new DirectoryEntry(entriesBytes.GetBytes(i, DirectoryEntry.Size), _isBigEndian);
                entry.Init();
                Entries[count] = entry;
                count++;
            }
        }
    }
}
