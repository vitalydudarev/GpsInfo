using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsInfo
{
    public class IFD
    {
        public short NumberOfDirectoryEntries;
        public int OffsetOfNextIfd;
        public DirectoryEntry[] Entries;
        public bool HasExifIfdPointer;

        public IFD(byte[] bytes, bool isBigEndian)
        {
            NumberOfDirectoryEntries = bytes.GetBytes(0, 2).ToInt16(isBigEndian);
            byte[] entriesBytes = bytes.GetBytes(2, NumberOfDirectoryEntries * DirectoryEntry.Size);
            OffsetOfNextIfd = bytes.GetBytes(2 + entriesBytes.Length, 4).ToInt32(isBigEndian);

            Entries = new DirectoryEntry[NumberOfDirectoryEntries];
            int count = 0;

            for (int i = 0; i < entriesBytes.Length; i = i + DirectoryEntry.Size)
            {
                var entry = new DirectoryEntry(entriesBytes.GetBytes(i, DirectoryEntry.Size), isBigEndian);
                Entries[count] = entry;
                count++;
            }
        }
    }
}
