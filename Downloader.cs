using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PicsDownloader
{
    public class Downloader
    {
        private static string m_path = Path.Combine(Program.AppPath, "pics.zip");
        private static bool m_downloaded = false;

        public void Start()
        {
            Thread updateThread = new Thread(InternalDownload) { IsBackground = true };
            updateThread.Start();
        }

        private void InternalDownload()
        {
            try
            {
                DownloadPics();
            }
            catch (Exception)
            {
                Program.Frm.SetText("Error when downloading.");
                Thread.Sleep(2000);
            }
        }

        public void DownloadPics()
        {
            if (!Directory.Exists(Path.Combine(Program.AppPath, "pics")))
                Directory.CreateDirectory(Path.Combine(Program.AppPath, "pics"));

            m_downloaded = false;
            using (var m_client = new WebClient())
            {
                m_client.DownloadProgressChanged += M_client_DownloadProgressChanged;
                m_client.DownloadFileCompleted += M_client_DownloadFileCompleted;
                m_client.DownloadFileAsync(new Uri("https://github.com/SuperAndroid17/DevPro-Images/archive/master.zip"), m_path);
            }
            while (!m_downloaded)
                Thread.Sleep(10);
        }

        public void ExtractPics(int attempt)
        {
            ZipFile zipfile = null;
            try
            {
                zipfile = new ZipFile(File.OpenRead(m_path));
            }
            catch
            {
                if (attempt == 3)
                {
                    if (MessageBox.Show("Error trying to extract pics, do you want to retry?", "Error Installing", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ExtractPics(0);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                Thread.Sleep(500);
                ExtractPics(attempt + 1);
                return;
            }

            for (int i = 0; i < zipfile.Count; i++)
            {
                ZipEntry entry = zipfile[i];
                if (!entry.IsFile) continue;

                double percent = (double)i / zipfile.Count;
                int percentInt = (int)(percent * 100);
                if (percentInt > 100) percentInt = 100;
                if (percentInt < 0) percentInt = 0;
                Program.Frm.SetText("Installing " + entry.Name);
                Program.Frm.SetProgress(percentInt);

                string filename = Path.Combine(Program.AppPath, entry.Name);
                string directory = Path.GetDirectoryName(filename);
                if (directory.Substring(directory.Length - 4).Trim() == "pics")
                {
                    string[] args = filename.Split('/');
                    string path = Path.Combine(Program.AppPath, "pics", args[args.Length - 1]);
                    byte[] buffer = new byte[4096];
                    Stream zipStream = zipfile.GetInputStream(entry);
                    using (FileStream streamWriter = new FileStream(path, FileMode.Create, FileAccess.Write))
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
            Program.Frm.SetProgress(100);

            zipfile.Close();

            File.Delete(Path.Combine(m_path));
            MessageBox.Show("It's ok ! Your launcher is ready to play. Good game.");
            Application.Exit();
        }

        private void M_client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            m_downloaded = true;
            Program.Frm.SetProgress(0);
            Program.Frm.SetText("Extracting pics...");
            ExtractPics(0);
        }

        private void M_client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Program.Frm.SetProgress(e.ProgressPercentage);
        }
    }
}
