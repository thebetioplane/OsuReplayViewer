using System.IO;
using FileEntry = System.Collections.Generic.KeyValuePair<string, ReplayViewer.Maintenance.MD5Hash>;

namespace ReplayViewer.Maintenance
{
    class ManifestFile
    {
        public int Version { get; private set; }
        public FileEntry[] Files;
        public ManifestFile(byte[] aob)
        {
            using (MemoryStream ms = new MemoryStream(aob))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                Version = reader.ReadInt32();
                int count = reader.ReadInt32();
                Files = new FileEntry[count];
                for (int i = 0; i < count; ++i)
                {
                    string name = reader.ReadString();
                    MD5Hash hash = reader.ReadBytes(16);
                    Files[i] = new FileEntry(name, hash);
                }
            }
        }
        public void SaveAs(string fname)
        {
            using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(Version);
                writer.Write(Files.Length);
                foreach (FileEntry item in Files)
                {
                    writer.Write(item.Key);
                    byte[] hash = item.Value;
                    writer.Write(hash);
                }
            }
        }
    }
}
