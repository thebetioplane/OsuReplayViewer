using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace ReplayViewer
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        private static bool shouldCheckUpdate = true;
        public const string BUILD_DATE = "2019-12-10";

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (File.Exists("no-update"))
                shouldCheckUpdate = true;
            if (ShouldRepair())
            {
                if (!DoRepair())
                    return;
            }
            Main2();
        }

        private static bool ShouldRepair()
        {
            string[] requiredDlls = {
                "ManagedBass.Fx.dll", "ManagedBass.dll", "OpenTK.GLControl.dll", "OpenTK.dll",  "bass.dll", "bass_fx.dll"
            };
            foreach (string name in requiredDlls)
            {
                if (!File.Exists(name))
                    return true;
            }
            return false;
        }

        private static bool DoRepair()
        {
            DialogResult res = MessageBox.Show("You are missing one or more DLL files required to run. Would you like to run the updater now? (it will download files in the current directory so you may want to move the .exe to it's own folder.\n\nPress 'no' to quit.", "OsuReplayViewer", MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes)
                return false;
            shouldCheckUpdate = false;
            using (Maintenance.Updater updater = new Maintenance.Updater())
            {
                updater.Run();
            }
            return true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Crash !");
            try
            {
                string d = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.WriteAllText("crash.log", d + "\r\n" + e.ExceptionObject.ToString());
            }
            catch
            {
                Console.WriteLine("An exception occured while writing crash.log");
            }
        }

        private static void Main2()
        {
            SetProcessDPIAware();
            string[] settings = new string[3];
            if (File.Exists(MainForm.Path_Settings))
            {
                settings = Program.LoadSettings();
            }
            else
            {
                settings = new string[]
                {
                    @"# Lines starting with a # are ignored",
                    @"# Do not change the order of the settings",
                    @"",
                    @"# Path to osu!.db",
                    @"C:\osu!\osu!.db",
                    @"",
                    @"# Path to your osu! song folder",
                    @"C:\osu!\songs\",
                    @"",
                    @"# Path to a replay folder",
                    @"C:\osu!\replays\"
                };
                File.WriteAllLines(MainForm.Path_Settings, settings);
                DialogResult reply = MessageBox.Show("A settings file has been created for you to link to your songs folder. Would you like to edit it now?", "File Created", MessageBoxButtons.YesNo);
                if (reply == DialogResult.Yes)
                {
                    Program.OpenSettings();
                    return;
                }
            }
            if (shouldCheckUpdate)
            {
                new Thread(() =>
                {
                    try
                    {
                        using (var updater = new Maintenance.Updater())
                        {
                            updater.Run();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }).Start();
            }
            using (MainForm form = new MainForm())
            {
                form.SetSettings(settings);
                Application.Run(form);
            }
            //form.Canvas = new Canvas(form.GetPictureBoxHandle(), form);
            //form.Canvas.Run();
        }


        public static void OpenSettings()
        {
            try
            {
                System.Diagnostics.Process.Start(MainForm.Path_Settings);
            }
            catch (Exception e)
            {
                MainForm.ErrorMessage(e.Message);
            }
        }

        public static string[] LoadSettings()
        {
            string[] settings = new string[3];
            string[] lines = File.ReadAllLines(MainForm.Path_Settings);
            int n = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (n >= settings.Length)
                {
                    break;
                }
                if (lines[i].Length > 0 && lines[i][0] != '#')
                {
                    settings[n] = lines[i];
                    n++;
                }
            }
            return settings;
        }
    }
}
