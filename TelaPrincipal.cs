using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damas.Apresentacao
{
    public partial class TelaPrincipal : Form
    {
        private SoundPlayer musicaTela;
        public Timer tmLabelTimer = new Timer();

        public TelaPrincipal()
        {
            InitializeComponent();
            InicializaMusica();

            var pos = this.PointToScreen(lbMultiplayer.Location);
            pos = pbTelaPrincipalBackground.PointToClient(pos);
            lbMultiplayer.Parent = pbTelaPrincipalBackground;
            lbMultiplayer.Location = pos;
            lbMultiplayer.BackColor = Color.Transparent;

            var pos1 = this.PointToScreen(lbTituloJogo.Location);
            pos1 = pbTelaPrincipalBackground.PointToClient(pos1);
            lbTituloJogo.Parent = pbTelaPrincipalBackground;
            lbTituloJogo.Location = pos1;
            lbTituloJogo.BackColor = Color.Transparent;

            var pos2 = this.PointToScreen(lbRanking.Location);
            pos2 = pbTelaPrincipalBackground.PointToClient(pos2);
            lbRanking.Parent = pbTelaPrincipalBackground;
            lbRanking.Location = pos2;
            lbRanking.BackColor = Color.Transparent;

            var pos4 = this.PointToScreen(lbSinglePlayer.Location);
            pos4 = pbTelaPrincipalBackground.PointToClient(pos4);
            lbSinglePlayer.Parent = pbTelaPrincipalBackground;
            lbSinglePlayer.Location = pos4;
            lbSinglePlayer.BackColor = Color.Transparent;

            var pos3 = this.PointToScreen(lbSairDoJogo.Location);
            pos3 = pbTelaPrincipalBackground.PointToClient(pos3);
            lbSairDoJogo.Parent = pbTelaPrincipalBackground;
            lbSairDoJogo.Location = pos3;
            lbSairDoJogo.BackColor = Color.Transparent;

            var pos5 = this.PointToScreen(pbInfo.Location);
            pos5 = pbTelaPrincipalBackground.PointToClient(pos5);
            pbInfo.Parent = pbTelaPrincipalBackground;
            pbInfo.Location = pos5;
            pbInfo.BackColor = Color.Transparent;

        }

        private void InicializaMusica()
        {
            musicaTela = new SoundPlayer();
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Jogo de Damas.wav"))
            {
                musicaTela.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "Jogo de Damas.wav";
                musicaTela.PlayLooping();
            }

        }

        private void lbSinglePlayer_Click(object sender, EventArgs e)
        {
            TelaModoJogo telaModoJogo = new TelaModoJogo();
            telaModoJogo.Show(this);
            //modoDeJogo = 1; //Single Player - Nível Fácil
            //JogoDama singleGame = new JogoDama(modoDeJogo);
            //singleGame.Show();
            this.Hide();
        }

        private void lbMultiplayer_Click(object sender, EventArgs e)
        {
            JogoDama jogo = new JogoDama();
            jogo.Show();
            this.Hide();
            //musicaTela.Stop();
        }

        private void lbRanking_Click(object sender, EventArgs e)
        {
            RankingBrowser ConsultaRanking = new RankingBrowser();
            ConsultaRanking.Location = new Point((this.Location.X / 2) + this.Width / 2 - ConsultaRanking.Width / 2,(this.Location.Y / 2) + this.Height / 2 - ConsultaRanking.Height / 2);
            //ConsultaRanking.Location = new Point(this.Location.X + this.Width / 2 - ConsultaRanking.Width / 2, this.Location.Y + this.Height / 2 - ConsultaRanking.Height / 2);
            ConsultaRanking.Show();
        }

        private void lbSairDoJogo_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TelaPrincipal_Load(object sender, EventArgs e)
        {
            tmLabelTimer.Start();
            tmLabelTimer.Enabled = true;
        }

        private void tmLabelTimer_Tick(object sender, EventArgs e)
        {
            Random rand = new Random();
            int A = rand.Next(0, 255);
            int B = rand.Next(0, 255);
            int C = rand.Next(0, 255);

            lbTituloJogo.ForeColor = Color.FromArgb(A, B, C);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Jogo de Damas - Trabalho de Engenharia de Software 2\t\n\t\n" +
                "7º Período - Curso de Engenharia da Computação\t\n\t\nUniversidade Estadual de Minas Gerais - Campus Divinópolis - 2016\t\n" +
                "Como jogar:\t\n" +
                "O jogo segue basicamente as regras das damas tradicionais, com exceção do fato que os peões não podem comer" +
                "para trás. Além disso, as damas não se movem em longa distância. A única vantagem de uma dama sobre uma peça normal é a " +
                "capacidade de se mover e capturar para trás, bem como para frente, uma casa do tabuleiro de cada vez. O peão e a dama têm " +
                "o mesmo valor para tomar ou ser tomada.\t\n\t\n" +
                "Mais informações: dama.semprenegocio.com.br/", "Sobre", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
