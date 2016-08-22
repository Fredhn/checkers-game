using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damas.Apresentacao
{
    public partial class RankingBrowser : Form
    {
        public RankingBrowser()
        {
            InitializeComponent();
        }

        private void toolStripButtonFind_Check_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripTextBoxSearch.Text)) return;
            if (toolStripTextBoxSearch.Text.Equals("about:blank")) return;
            if (!toolStripTextBoxSearch.Text.StartsWith("http://")
                && !toolStripTextBoxSearch.Text.StartsWith("https://"))
            {
                toolStripTextBoxSearch.Text = "http://" + toolStripTextBoxSearch.Text;
            }
            try
            {
                webBrowserRanking.Navigate(new Uri(toolStripTextBoxSearch.Text));
            }
            catch (System.UriFormatException)
            {
                return;
            }

        }
        private void webBrowserRanking_Atualiza (object sender, WebBrowserNavigatedEventArgs e)
        {
            toolStripTextBoxSearch.Text = webBrowserRanking.Url.ToString();
        }

        private void toolStripButtonBackwards_Click(object sender, EventArgs e)
        {
            webBrowserRanking.GoBack();
        }

        private void toolStripButtonForward_Click(object sender, EventArgs e)
        {
            webBrowserRanking.GoForward();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            webBrowserRanking.Refresh();
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            webBrowserRanking.Stop();
        }

        private void toolStripButtonHome_Click(object sender, EventArgs e)
        {
            webBrowserRanking.GoHome();
            //string url = "http://dama.expressahost.com.br";
            //System.Diagnostics.Process.Start(url);
        }

        private void toolStripTextBoxSearch_Click(object sender, EventArgs e)
        {
        }
    }
}
