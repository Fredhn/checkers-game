using Damas.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Media;

namespace Damas.Apresentacao
{
    public partial class TelaResultado : Form
    {
        public string nomeVencedor;
        private string timerVencedor;
        private int movimentosVencedor;
        private int scoreVencedor;
        private int vencedor_ID = 0;
        private int modoDeJogo = 0;

        public TelaResultado()
        {
            InitializeComponent();
            InicializaMusicaFinal();
            this.FormClosing += FinalizaJogo_FinalizaJogoClosing;

        }

        public TelaResultado(string timerVencedor, int movimentosVencedor, int scoreVencedor, int vencedor, int modoJogo)
        {
            InitializeComponent();
            InicializaMusicaFinal();
            this.FormClosing += FinalizaJogo_FinalizaJogoClosing;
            this.timerVencedor = timerVencedor;
            this.movimentosVencedor = movimentosVencedor;
            this.scoreVencedor = scoreVencedor;
            this.vencedor_ID = vencedor;
            this.modoDeJogo = modoJogo;
            ExibeVencedor();
            if (vencedor_ID == 2 && modoDeJogo == 1 || vencedor_ID == 2 && modoDeJogo == 2)
            {
                lbInformarNome.Hide();
                btnScoreSubmit.Hide();
                tbNomeJogador.Hide();
                btnSair.Show();
            }
        }

        public void ExibeVencedor()
        {
            lbMostraVencedorID.Text = vencedor_ID.ToString();
        }

        private void InicializaMusicaFinal()
        {
            SoundPlayer musicaFinal = new SoundPlayer();
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "GanhaJogo.wav"))
            {
                musicaFinal.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "GanhaJogo.wav";
                musicaFinal.Play();
            }

        }

        public static bool verificaConexaoInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var conexao = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    } 
                }
            }
            catch
            {
                return false;
            }
        }

        private void FinalizaJogo_Load(object sender, EventArgs e)
        {
        
        }

        private void btnScoreSubmit_Click(object sender, EventArgs e)
        {
            this.nomeVencedor =  this.tbNomeJogador.Text;

            if (verificaConexaoInternet())
            {
                WebApi.EnviaDadosAposJogo(this.nomeVencedor, this.scoreVencedor, this.timerVencedor, this.movimentosVencedor);
                MessageBox.Show("Registro inserido no ranking! Clique em Ok para continuar.");
                this.Close();
                Application.Restart();

            }
            else
            {
                MessageBox.Show("Falha de conexão! Clique em Ok para continuar.");
                this.Close();
                Application.Restart();
            }
        }

        private void FinalizaJogo_FinalizaJogoClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Tem certeza que deseja fechar o jogo sem se registrar no ranking?", "Fechando...", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    e.Cancel = false;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void FinalizaJogo_FinalizaJogoCancel(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Tem certeza que deseja fechar essa tela?", "Fechando...",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Restart();
            }
        }

        private void lbFinalizaJogo_Click(object sender, EventArgs e)
        {

        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void lbFinalizaJogoVencedor_Click(object sender, EventArgs e)
        {

        }
    }
}
