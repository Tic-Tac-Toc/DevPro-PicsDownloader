using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicsDownloader
{
    public partial class Download_frm : Form
    {
        public Download_frm()
        {
            InitializeComponent();
            Load += Download_frm_Load;
        }

        private void Download_frm_Load(object sender, EventArgs e)
        {
            Program.Downloader.Start();
        }

        public void SetText(string text)
        {
            Invoke(new Action<string>(InternalSetText), text);
        }

        private void InternalSetText(string text)
        {
            lblText.Text = text;
        }

        public void SetProgress(int percent)
        {
            Invoke(new Action<int>(InternalSetProgress), percent);
        }

        private void InternalSetProgress(int progress)
        {
            pbPicsDownload.Value = progress;
            lblProgress.Text = progress.ToString() + "%";
        }

        public void Exit()
        {
            Invoke(new Action(Close));
        }
    }
}