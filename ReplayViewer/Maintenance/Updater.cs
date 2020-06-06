using System;
using System.IO;
using System.Net;
using FileEntry = System.Collections.Generic.KeyValuePair<string, ReplayViewer.Maintenance.MD5Hash>;

namespace ReplayViewer.Maintenance
{
    class Updater : IDisposable
    {
        private const string PREFIX = "https://raw.githubusercontent.com/thebetioplane/OsuReplayViewer/master/distro/";
        private readonly string TIMESTAMP = "?t=" + DateTime.Now.Ticks.ToString();
        private const string LOCAL_MFILE = "manifest";
        private WebClient Client = new WebClient();
        public bool IsRunning { get; private set; }

        public void Run()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("the updater is already running");
            IsRunning = true;
            try
            {
                Go();
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void Go()
        {
            if (! File.Exists(LOCAL_MFILE))
            {
                GetFile("manifest");
            }
            if (! Directory.Exists("img"))
            {
                Directory.CreateDirectory("img");
            }
            ManifestFile localM = new ManifestFile(File.ReadAllBytes(LOCAL_MFILE));
            ManifestFile webM = new ManifestFile(GetData("manifest"));
            Console.WriteLine("local version {0}, web version {1}", localM.Version, webM.Version);
            bool gotAtLeastOneFile = false;
            foreach (FileEntry ent in localM.Files)
            {
                File.Delete(ent.Key + ".swp");
                if (! File.Exists(ent.Key) || ent.Value.CompareTo(ComputeHash(ent.Key)) != 0)
                {
                    gotAtLeastOneFile = true;
                    GetFile(ent.Key);
                }
            }
            if (gotAtLeastOneFile)
            {
                webM.SaveAs(LOCAL_MFILE);
                System.Windows.Forms.MessageBox.Show("A new version of OsuReplayViewer has been downloaded. Close and reopen the program to take effect.", "new update downloaded");
            }
        }

        private MD5Hash ComputeHash(string fname)
        {
            try
            {
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                {
                    return new MD5Hash(fs);
                }
            }
            catch
            {
                return null;
            }
        }

        private void GetFile(string fname)
        {
            string url = PREFIX + fname + TIMESTAMP;
            string dlFname = fname + ".dl";
            string swpFname = fname + ".swp";
            Client.DownloadFile(url, dlFname);
            MoveIfExist(fname, swpFname);
            MoveIfExist(dlFname, fname);
        }

        private byte[] GetData(string fname)
        {
            string url = PREFIX + fname + TIMESTAMP;
            return Client.DownloadData(url);
        }

        private void MoveIfExist(string a, string b)
        {
            try
            {
                File.Delete(b);
                if (File.Exists(a))
                    File.Move(a, b);
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            this.Client?.Dispose();
        }
    }
}
