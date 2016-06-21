using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace ReplayViewer
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
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
            MainForm form = new MainForm();
            form.SetSettings(settings);
            form.Show();
            form.Canvas = new Canvas(form.GetPictureBoxHandle(), form);
            form.Canvas.Run();
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
