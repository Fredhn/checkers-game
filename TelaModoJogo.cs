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
    public partial class TelaModoJogo : Form
    {
        public int modoDeJogo = 0;

        public TelaModoJogo()
        {
            InitializeComponent();
            //FormBorderStyle = FormBorderStyle.None;
            //this.TransparencyKey = this.BackColor = Color.Fuchsia;
            //this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //this.Location = new Point((this.Owner.Width - this.Width) / 2, (this.Owner.Height - this.Height) / 2);
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    this.DialogResult = DialogResult.OK;
        //}

        private void btnModoFacil_Click(object sender, EventArgs e)
        {
            modoDeJogo = 1; //Single Player - Nível Fácil
            JogoDama singleGameEasy = new JogoDama(modoDeJogo);
            singleGameEasy.Show();
            this.Close();
        }

        private void btnModoDificil_Click(object sender, EventArgs e)
        {
            modoDeJogo = 2; //Single Player - Nível Dificil
            JogoDama singleGameHard = new JogoDama(modoDeJogo);
            singleGameHard.Show();
            this.Close();
        }

        private void TelaModoJogo_Load(object sender, EventArgs e)
        {

        }
    }
}
