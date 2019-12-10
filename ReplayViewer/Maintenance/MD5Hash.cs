using System;
using System.IO;

namespace ReplayViewer.Maintenance
{
    class MD5Hash : IComparable<MD5Hash>
    {
        public byte[] Hash { get; private set; }
        public MD5Hash(Stream stream)
        {
            using (var m = System.Security.Cryptography.MD5.Create())
            {
                this.Hash = m.ComputeHash(stream);
            }
        }

        private MD5Hash()
        {
            this.Hash = new byte[16];
        }

        public static implicit operator MD5Hash(byte[] aob)
        {
            if (aob.Length != 16)
                throw new ArgumentException("MD5 hashes must have a length of 16 bytes");
            MD5Hash obj = new MD5Hash();
            for (int i = 0; i < 16; ++i)
                obj.Hash[i] = aob[i];
            return obj;
        }

        public static implicit operator byte[] (MD5Hash obj)
        {
            return obj.Hash;
        }

        public int CompareTo(MD5Hash other)
        {
            if (other == null)
                return -1;
            for (int i = 0; i < 16; ++i)
            {
                int diff = Hash[i] - other.Hash[i];
                if (diff != 0)
                    return diff;
            }
            return 0;
        }
    }
}