using System;
using System.IO;
using System.Collections.Generic;
using MD5 = System.Security.Cryptography.MD5;

namespace ManifestGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<string> fileList = new List<string>();
            int version = -1;
            const string FILELIST_TXT = "filelist.txt";
            if (! File.Exists(FILELIST_TXT))
            {
                Console.WriteLine("there is no file named '{0}'", FILELIST_TXT);
                return;
            }
            using (FileStream fs = new FileStream(FILELIST_TXT, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                version = Convert.ToInt32(sr.ReadLine());
                while (!sr.EndOfStream)
                {
                    string name = sr.ReadLine();
                    if (name.Length <= 1)
                        continue;
                    if (!File.Exists(name))
                    {
                        Console.WriteLine("file does not exist: {0}", name);
                        return;
                    }
                    fileList.Add(name);
                }
            }
            using (FileStream fs = new FileStream("manifest", FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(version);
                writer.Write(fileList.Count);
                foreach (string file in fileList)
                {
                    byte[] hash = MD5File(file);
                    if (hash != null)
                    {
                        writer.Write(file);
                        writer.Write(hash);
                    }
                }
            }
        }

        private static byte[] MD5File(string fname)
        {
            try
            {
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                using (MD5 md5 = MD5.Create())
                {
                    return md5.ComputeHash(fs);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}