using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Damas
{

    public class CasaTabuleiro
    {
        private JogoDama jogoDataForm;
        public PictureBox Imagem { get; set; }
        public bool Nulo { get; set; } //Casa vazia = True
        public int ID { get; set; } //ID do jogador
        public int Estado
        {
            get
            {
                return _estado;
            }
            set
            {
                _estado = value;
                AtualizaImage();
            }
        }
        private int _estado;

        public CasaTabuleiro (int i, int j, JogoDama jogoDataForm)
        {
            this.jogoDataForm = jogoDataForm;
            CriaPictureBox(i, j);
            InicializaPecas(i);
        }

        public CasaTabuleiro()
        {
        }

        public void CriaPictureBox(int i, int j)
        {
            this.Imagem = new PictureBox();
            this.Imagem.Size = new Size(50, 50);
            this.Imagem.Location = new Point((int)j * this.Imagem.Width, (int)i * this.Imagem.Height);
            if ((i + 1) % 2 > 0)
            {
                if ((j + 1) % 2 > 0)
                {
                    this.Imagem.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    this.Nulo = true;
                }
                else
                {
                    this.Imagem.BackColor = ColorTranslator.FromHtml("#444444");
                    this.Nulo = false;
                }
            }
            else
            {
                if ((j + 1) % 2 > 0)
                {
                    this.Imagem.BackColor = ColorTranslator.FromHtml("#444444");
                    this.Nulo = false;
                }
                else
                {
                    this.Imagem.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    this.Nulo = true;
                }
            }
            if (!this.Nulo) this.Imagem.Click += new EventHandler(this.Peca_Click);
        }

        public void InicializaPecas(int i)
        {
            //PECAS JOGADOR 01
            if (i <= 2 && !this.Nulo)
            {
                this.Estado = 1;
                this.ID = 1;
            }
            //PECAS JOGADOR 02
            else if (i >= 5 && !this.Nulo)
            {
                this.Estado = 2;
                this.ID = 2;
            }
            //CAMPOS SEM PECAS
            else
            {
                this.Estado = 0;
                this.ID = 0;
            }
        }

        public void AtualizaImage()
        {
            switch (this._estado)
            {
                case 0://SEM PECA
                    this.ID = 0;
                    this.Imagem.BackgroundImage = null;
                    this.Imagem.BackgroundImageLayout = ImageLayout.None;
                    break;
                case 1://PEÃO JOGADOR 1
                    this.ID = 1;
                    this.Imagem.BackgroundImage = Properties.Resources.peca_1;
                    this.Imagem.BackgroundImageLayout = ImageLayout.Center;
                    break;
                case 2://PEÃO JOGADOR 2
                    this.ID = 2;
                    this.Imagem.BackgroundImage = Properties.Resources.peca_2;
                    this.Imagem.BackgroundImageLayout = ImageLayout.Center;
                    break;
                case 3://DAMA JOGADOR 1
                    this.ID = 1;
                    this.Imagem.BackgroundImage = Properties.Resources.peca_1_dama;
                    this.Imagem.BackgroundImageLayout = ImageLayout.Center;
                    break;
                case 4://DAMA JOGADOR 2
                    this.ID = 2;
                    this.Imagem.BackgroundImage = Properties.Resources.peca_2_dama;
                    this.Imagem.BackgroundImageLayout = ImageLayout.Center;
                    break;
            }
        }

        public void Peca_Click(object sender, EventArgs e)
        {
            jogoDataForm.PecaSelecionada(this);
        }

        /*Encontra indíce da picture box selecionada: 
        * i = Borda Superior da Imagem(coordenada em pixel)/Altura da Imagem(em pixel)
        * j = Borda Esquerda da Imagem(coordenada em pixel)/Largura da Imagem(em pixel)
        */
        public int i{ get { return (this.Imagem != null) ? this.Imagem.Top / this.Imagem.Height : 0; } }
        public int j{ get { return (this.Imagem != null) ? this.Imagem.Left / this.Imagem.Width : 0; } }
    }
}
