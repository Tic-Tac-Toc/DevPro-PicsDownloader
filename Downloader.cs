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
            catch (Exception ex)
            {
                Program.Frm.SetText("Error when downloading.");
                File.WriteAllText(Path.Combine(Program.AppPath, "Logs", "PicsDownloader", "error_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log"), ex.ToString());
                Thread.Sleep(2000);
            }
        }

        public void DownloadPics()
        {
            if (!Directory.Exists(Path.Combine(Program.AppPath, "Logs", "PicsDownloader")))
                Directory.CreateDirectory(Path.Combine(Program.AppPath, "Logs", "PicsDownloader"));
             
            if (!Directory.Exists(Path.Combine(Program.AppPath, "pics")))
            {
                Directory.CreateDirectory(Path.Combine(Program.AppPath, "pics"));
                Directory.CreateDirectory(Path.Combine(Program.AppPath, "pics", "field"));
                Directory.CreateDirectory(Path.Combine(Program.AppPath, "pics", "thumbnail"));
            }
            if (File.Exists("pics.zip"))
                ExtractPics(0);
            else
            {

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
        }

        public void ExtractPics(int attempt)
        {
            ZipFile zipfile = null;
            try
            {
                zipfile = new ZipFile(File.OpenRead(m_path));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to extract pics, do you want to retry?", "Error Installing");
                File.WriteAllText(Path.Combine(Program.AppPath, "Logs", "PicsDownloader", "error_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log"), ex.ToString());
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
                Program.Frm.SetProgress(percentInt);

                string[] directories = entry.Name.Split('/');
                string filename = entry.Name.Substring(directories[0].Length + 1).Trim();
                string directory = Path.GetDirectoryName(filename);
                Program.Frm.SetText("Extracting " + filename);

                if (directory != "")
                {
                    if(!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    byte[] buffer = new byte[4096];
                    Stream zipStream = zipfile.GetInputStream(entry);
                    using (FileStream streamWriter = new FileStream(filename, FileMode.Create, FileAccess.Write))
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
