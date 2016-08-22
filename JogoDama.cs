using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using Newtonsoft.Json;
using Damas.Core;
using Damas.Apresentacao;
using System.Threading;

namespace Damas
{
    public partial class JogoDama : Form
    {
        public CasaTabuleiro[,] tabuleiro;

        public string[] movimentoValido;

        public List<Coordenada> _coordenadasValidadas;

        public int contadorTurno = 0;

        public int jogadorTurno = 0;

        public int vencedor_ID = 0;

        public string timerVencedor;

        public int movimentosVencedor;

        public int scoreVencedor;

        public bool musicaTocando;

        private SoundPlayer musica;

        public int modoJogo = 0;

        public JogoDama()
        {
            InitializeComponent();

            MontaTabuleiro();
            this.FormClosing += JogoDama_JogoDamaClosing_2;
            InicializaMusica();
        }

        public JogoDama(int modoDeJogo)
        {
            InitializeComponent();
            tmGame.Stop();
            this.modoJogo = modoDeJogo;

            if(!VerificaContinuacaoJogo())
                MontaTabuleiro();

            tmGame.Start();

            this.FormClosing += JogoDama_JogoDamaClosing_2;
            InicializaMusica();
        }

        #region MÉTODOS CONTROLADORES DO AMBIENTE
        private void InicializaMusica()
        {
            musica = new SoundPlayer();
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Jogo de Damas 2.wav"))
            {
                musica.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "Jogo de Damas 2.wav";
                musica.PlayLooping();
                musicaTocando = true;
            }

        }

        public void MontaTabuleiro(int[,] preEstados = null)
        {
            tabuleiro = new CasaTabuleiro[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tabuleiro[i, j] = new CasaTabuleiro(i, j, this);

                    if (preEstados != null)
                        tabuleiro[i, j].Estado = preEstados[i, j];

                    this.pnTabuleiro.Controls.Add(tabuleiro[i, j].Imagem);
                }
            }
            if (modoJogo == 1 || modoJogo == 2)
            {
                jogadorTurno = 1;
                pnJogador1.BackColor = ColorTranslator.FromHtml("#324750");
            }
            else
            {
                SorteiaTurnos();
            }
        }

        public void NomalizaBackGroundsPecas()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!tabuleiro[i, j].Nulo)
                        tabuleiro[i, j].Imagem.BackColor = ColorTranslator.FromHtml("#444");
                }
            }
        }
        #endregion

        //EVENTO DE CLICK: PEÇAS DO TABULEIRO
        public void PecaSelecionada(CasaTabuleiro peca)
        {
            int _pecaEstado = peca.Estado;
            int _ID = peca.ID;
            int _i = peca.i;
            int _j = peca.j;
            NomalizaBackGroundsPecas();
            /*Verifica se o turno pertence ao jogador que clicou na sua peça para validar os movimentos desta peça*/
            if (_ID == jogadorTurno && peca.Estado >= 1)
            {
                peca.Imagem.BackColor = ColorTranslator.FromHtml("#222");
                ValidaMovimentos(peca);
                SinalizaMovimentos(_coordenadasValidadas);

            }
            /*Se a peça possuir movimentos válidos, a funcionalidade de pular casas/comer peças é habilitada*/
            if (_coordenadasValidadas != null && _coordenadasValidadas.Count > 0)
            {
                PecaPula(peca, _coordenadasValidadas);

                /*Se o jogador acabou de comer uma peça adversária e ainda possui movimentos válidos para comer outra peça,
                  a funcionalidade de comer peças é novamente habilitada*/
                if (_coordenadasValidadas.Where(coordenada => coordenada.come_peca != null).Count() > 0)
                {
                    SinalizaMovimentos(_coordenadasValidadas);
                    PecaPula(peca, _coordenadasValidadas);
                }
            }
        }

        #region MÉTODO PULAR CASA / COMER PEÇA
        public void PecaPula(CasaTabuleiro peca, List<Coordenada> movimentoValido)
        {
            int _i = peca.i;
            int _j = peca.j;
            int _estado = peca.Estado;
            var coordenada = movimentoValido.Where(coord => coord.x == _i
                                                && coord.y == _j).FirstOrDefault();

            //Executa o movimento: come a peça do adversário com peão/dama do jogador 1
            if (movimentoValido.Where(coord => coord.x == _i
                                            && coord.y == _j
                                            && coord.come_peca != null
                                            && (coord.peca_jogador == 1 || coord.peca_jogador == 3)).Count() > 0)
            {
                /*Seta a casa onde o jogador clicou e está na lista de movimentos válidos com o estado da peça que vai se mudar para esta casa.*/
                tabuleiro[_i, _j].Estado = coordenada.peca_jogador;
                /*Seta a casa da peça que foi comida como vazia.*/
                tabuleiro[coordenada.come_peca[0], coordenada.come_peca[1]].Estado = 0;
                /*Seta a casa de origem da peça que foi movimentada como vazia.*/
                tabuleiro[coordenada.coord_pai[0], coordenada.coord_pai[1]].Estado = 0;

                ValidaMovimentos(peca);
                if (_coordenadasValidadas.Where(cda => cda.come_peca != null).Count() == 0)
                {

                    InverteTurno();
                }
            }
            //Executa o movimento: come a peça do adversário com peão/dama do jogador 2
            else if (movimentoValido.Where(coord => coord.x == _i
                                            && coord.y == _j
                                            && coord.come_peca != null
                                            && (coord.peca_jogador == 2 || coord.peca_jogador == 4)).Count() > 0)
            {
                /*Seta a casa onde o jogador clicou e está na lista de movimentos válidos com o estado da peça que vai se mudar para esta casa.*/
                tabuleiro[_i, _j].Estado = coordenada.peca_jogador;
                /*Seta a casa da peça que foi comida como vazia.*/
                if (coordenada.come_peca != null)
                {
                    tabuleiro[coordenada.come_peca[0], coordenada.come_peca[1]].Estado = 0;
                }
                /*Seta a casa de origem da peça que foi movimentada como vazia.*/
                tabuleiro[coordenada.coord_pai[0], coordenada.coord_pai[1]].Estado = 0;

                ValidaMovimentos(peca);
                if (_coordenadasValidadas.Where(cda => cda.come_peca != null).Count() == 0)
                {

                    InverteTurno();
                }
            }
            //Executa o movimento: não come a peça do adversário com peão/dama do jogador 1
            else if (movimentoValido.Where(coord => coord.x == _i
                                            && coord.y == _j
                                            && coord.come_peca == null
                                            && (coord.peca_jogador == 1 || coord.peca_jogador == 3)).Count() > 0)
            {
                /*Seta a casa onde o jogador clicou e está na lista de movimentos válidos com o estado da peça que vai se mudar para esta casa.*/
                tabuleiro[_i, _j].Estado = coordenada.peca_jogador;
                /*Seta a casa de origem da peça que foi movimentada como vazia.*/
                tabuleiro[coordenada.coord_pai[0], coordenada.coord_pai[1]].Estado = 0;

                InverteTurno();
            }
            //Executa o movimento: não come a peça do adversário com peão/dama do jogador 2
            else if (movimentoValido.Where(coord => coord.x == _i
                                            && coord.y == _j
                                            && coord.come_peca == null
                                            && (coord.peca_jogador == 2 || coord.peca_jogador == 4)).Count() > 0)
            {
                /*Seta a casa onde o jogador clicou e está na lista de movimentos válidos com o estado da peça que vai se mudar para esta casa.*/
                tabuleiro[_i, _j].Estado = coordenada.peca_jogador;
                /*Seta a casa de origem da peça que foi movimentada como vazia.*/
                tabuleiro[coordenada.coord_pai[0], coordenada.coord_pai[1]].Estado = 0;

                InverteTurno();
            }
        }
        #endregion

        #region MÉTODOS CONTROLADORES DO JOGO

        //Sorteia o jogador que inicia as jogadas
        public void SorteiaTurnos()
        {
            if (contadorTurno == 0)
            {
                Random turnoJogador = new Random();
                jogadorTurno = turnoJogador.Next(1, 3); //Sorteia um valor entre 1 e 2.
                InverteTurno(true);
            }
        }

        public void InverteTurno(bool inicializandoGame = false)
        {
            if (jogadorTurno == 1)
            {
                jogadorTurno = jogadorTurno + 1;
                lbMovimentosQtd1.Text = string.Format("{0:00}", (int.Parse(lbMovimentosQtd1.Text) + 1));
                pnJogador1.BackColor = pnJogador2.BackColor;
                pnJogador2.BackColor = ColorTranslator.FromHtml("#503232");
                if (modoJogo == 1)
                {
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    PcVerificaPecasEasy();
                }
                else if (modoJogo == 2)
                {
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    PcVerificaPecasNormal();
                }
            }
            else
            {
                jogadorTurno = jogadorTurno - 1;
                lbMovimentosQtd2.Text = string.Format("{0:00}", (int.Parse(lbMovimentosQtd2.Text) + 1));
                pnJogador2.BackColor = pnJogador1.BackColor;
                pnJogador1.BackColor = ColorTranslator.FromHtml("#324750");
            }
            if (!inicializandoGame)
            {
                lbTurnosQtd.Text = string.Format("{0:00}", (int.Parse(lbTurnosQtd.Text) + 1));
                ContaPecas();
            }
        }

        public void ContaPecas()
        {
            int qtdPecasJogador1 = 0;
            int qtdPecasJogador2 = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tabuleiro[i, j].ID == 1)
                    {
                        qtdPecasJogador1++;
                    }
                    else if (tabuleiro[i, j].ID == 2)
                    {
                        qtdPecasJogador2++;
                    }
                }
            }
            lbPecasQtd1.Text = string.Format("{0:00}", qtdPecasJogador1);
            lbPecasQtd2.Text = string.Format("{0:00}", qtdPecasJogador2);

            /*Condicionais que verificam se o jogo acabou*/
            if (qtdPecasJogador1 == 0)
            {
                vencedor_ID = 2;
                TerminarJogo();

            }
            else if (qtdPecasJogador2 == 0)
            {
                vencedor_ID = 1;
                TerminarJogo();

            }
            else if (qtdPecasJogador1 == 1 && _coordenadasValidadas.Count == 0)
            {
                vencedor_ID = 2;
                TerminarJogo();

            }
            else if (qtdPecasJogador2 == 1 && _coordenadasValidadas.Count == 0)
            {
                vencedor_ID = 1;
                TerminarJogo();

            }
        }

        public void TerminarJogo()
        {

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "JogoDamasSaveGame.log"))
            {
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\" + "JogoDamasSaveGame.log");
            }
            ContabilizaPontuacao();
            TelaResultado telaFinal = new TelaResultado(timerVencedor, movimentosVencedor, scoreVencedor, vencedor_ID, modoJogo);
            telaFinal.Show();
            musica.Stop();

        }

        public void ContabilizaPontuacao()
        {
            tmGame.Stop();
            if (vencedor_ID == 1)
            {
                timerVencedor = "00:" + lbMinutos1.Text + lbDoisPontos1.Text + lbSegundos1.Text;
                movimentosVencedor = int.Parse(lbMovimentosQtd1.Text);
                scoreVencedor = (((int.Parse(lbSegundos1.Text) * 1000) / (int.Parse(lbMinutos1.Text) + 1)) / int.Parse(lbMovimentosQtd1.Text));
            }
            else
            {
                timerVencedor = "00:" + lbMinutos2.Text + lbDoisPontos2.Text + lbSegundos2.Text;
                movimentosVencedor = int.Parse(lbMovimentosQtd2.Text);
                scoreVencedor = (((int.Parse(lbSegundos2.Text) * 1000) / (int.Parse(lbMinutos2.Text) + 1)) / int.Parse(lbMovimentosQtd2.Text));
            }
        }

        /*Timer: Controladores*/
        private void tmGame_Tick(object sender, EventArgs e)
        {
            int segundos = int.Parse(lbSegundos.Text);
            int minutos = int.Parse(lbMinutos.Text);
            if (segundos + 1 >= 60)
            {
                lbSegundos.Text = "00";
                lbMinutos.Text = string.Format("{0:00}", (minutos + 1));
            }
            else
                lbSegundos.Text = string.Format("{0:00}", (segundos + 1));

            if (jogadorTurno == 1)
            {
                segundos = int.Parse(lbSegundos1.Text);
                minutos = int.Parse(lbMinutos1.Text);
                if (segundos + 1 >= 60)
                {
                    lbSegundos1.Text = "00";
                    lbMinutos1.Text = string.Format("{0:00}", (minutos + 1));
                }
                else
                    lbSegundos1.Text = string.Format("{0:00}", (segundos + 1));
            }
            else
            {
                segundos = int.Parse(lbSegundos2.Text);
                minutos = int.Parse(lbMinutos2.Text);
                if (segundos + 1 >= 60)
                {
                    lbSegundos2.Text = "00";
                    lbMinutos2.Text = string.Format("{0:00}", (minutos + 1));
                }
                else
                    lbSegundos2.Text = string.Format("{0:00}", (segundos + 1));
            }
        }

        private void pbControleSom_Click(object sender, EventArgs e)
        {
            if (musicaTocando == true)
            {
                musica.Stop();
                musicaTocando = false;
                pbControleSom.Image = Properties.Resources.Disable_Music;
            }
            else
            {
                musica.Play();
                musicaTocando = true;
                pbControleSom.Image = Properties.Resources.Enable_Music;
            }
        }
        #endregion

        #region MÉTODO DE VALIDACÃO DE MOVIMENTOS
        public void ValidaMovimentos(CasaTabuleiro peca)
        {
            int _i = peca.i;
            int _j = peca.j;
            int _estado = peca.Estado;
            int _ID = peca.ID;
            _coordenadasValidadas = new List<Coordenada>();

            /*
            INICIO DA VALIDAÇÃO DE MOVIMENTOS
            */
            if (_estado == 1 || _estado == 3 || _estado == 4)
            {
                /*
                INÍCIO BAIXO DIREITA
                */

                //Movimento: Baixo direita - Jogador 1 (peão/dama) e Jogador 2 (dama) - Simples
                if (_i + 1 <= 6
                   && _j + 1 <= 7
                   && tabuleiro[_i + 1, _j + 1].Estado == 0)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 1 (peão) - Transforma dama
                else if (_i + 1 == 7
                        && _j + 1 <= 7
                        && tabuleiro[_i + 1, _j + 1].Estado == 0
                        && _estado == 1)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direta - Jogador 1 (Dama) e Jogador 2 (Dama) - Não transforma dama
                else if (_i + 1 == 7
                        && _j + 1 <= 7
                        && tabuleiro[_i + 1, _j + 1].Estado == 0
                        && (_estado == 3 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 1 (peão/dama) - Come peça
                else if (_i + 2 <= 6
                        && _j + 2 <= 7
                        && (tabuleiro[_i + 1, _j + 1].Estado == 2 || tabuleiro[_i + 1, _j + 1].Estado == 4)
                        && tabuleiro[_i + 2, _j + 2].Estado == 0
                        && (_estado == 1 || _estado == 3))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 2 (dama) - Come peça
                else if (_i + 2 <= 6
                        && _j + 2 <= 7
                        && (tabuleiro[_i + 1, _j + 1].Estado == 1 || tabuleiro[_i + 1, _j + 1].Estado == 3)
                        && tabuleiro[_i + 2, _j + 2].Estado == 0
                        && (_estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 1 (peão) - Come peça na extremidade inferior e transforma dama
                else if (_i + 2 == 7
                        && _j + 2 <= 7
                        && (tabuleiro[_i + 1, _j + 1].Estado == 2 || tabuleiro[_i + 1, _j + 1].Estado == 4)
                        && tabuleiro[_i + 2, _j + 2].Estado == 0
                        && _estado == 1)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 1 (dama) - Come peça na extremidade inferior 
                else if (_i + 2 == 7
                        && _j + 2 <= 7
                        && (tabuleiro[_i + 1, _j + 1].Estado == 2 || tabuleiro[_i + 1, _j + 1].Estado == 4)
                        && tabuleiro[_i + 2, _j + 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo direita - Jogador 2 (dama) - Come peça na extremidade inferior
                else if (_i + 2 == 7
                        && _j + 2 <= 7
                        && (tabuleiro[_i + 1, _j + 1].Estado == 1 || tabuleiro[_i + 1, _j + 1].Estado == 3)
                        && tabuleiro[_i + 2, _j + 2].Estado == 0
                        && _estado == 4)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                /*
                FIM BAIXO DIREITA
                */

                /*
                INÍCIO BAIXO ESQUERDA
                */

                //Movimento: Baixo esquerda - Jogador 1 (peão/dama) e Jogador 2 (dama) - Simples
                if (_i + 1 <= 6
                   && _j - 1 <= 7
                   && _j - 1 >= 0
                   && tabuleiro[_i + 1, _j - 1].Estado == 0)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                             ,
                        y = (_j - 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 1 (peão) - Transforma dama
                else if (_i + 1 == 7
                   && _j - 1 <= 7
                   && _j - 1 >= 0
                   && tabuleiro[_i + 1, _j - 1].Estado == 0
                   && _estado == 1)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                            ,
                        y = (_j - 1)
                                                            ,
                        peca_jogador = (_estado + 2)
                                                            ,
                        come_peca = null
                                                            ,
                        coord_pai = new int[] { _i, _j }
                                                            ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 1 (Dama) e Jogador 2 (Dama) - Não transforma dama
                else if (_i + 1 == 7
                        && _j - 1 <= 7
                        && _j - 1 >= 0
                        && tabuleiro[_i + 1, _j - 1].Estado == 0
                        && (_estado == 3 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 1)
                                                             ,
                        y = (_j - 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 1 (peão/dama) - Come peça
                else if (_i + 2 <= 6
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i + 1, _j - 1].Estado == 2 || tabuleiro[_i + 1, _j - 1].Estado == 4)
                        && tabuleiro[_i + 2, _j - 2].Estado == 0
                        && (_estado == 1 || _estado == 3))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 2 (dama) - Come peça
                else if (_i + 2 <= 6
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i + 1, _j - 1].Estado == 1 || tabuleiro[_i + 1, _j - 1].Estado == 3)
                        && tabuleiro[_i + 2, _j - 2].Estado == 0
                        && _estado == 4)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 1 (peão) - Come peça na extremidade inferior e transforma dama
                else if (_i + 2 == 7
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i + 1, _j - 1].Estado == 2 || tabuleiro[_i + 1, _j - 1].Estado == 4)
                        && tabuleiro[_i + 2, _j - 2].Estado == 0
                        && _estado == 1)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                            ,
                        y = (_j - 2)
                                                            ,
                        peca_jogador = (_estado + 2)
                                                            ,
                        come_peca = new int[] { (_i + 1), (_j - 1) }
                                                            ,
                        coord_pai = new int[] { _i, _j }
                                                            ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 1 (dama) - Come peça na extremidade inferior
                else if (_i + 2 == 7
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i + 1, _j - 1].Estado == 2 || tabuleiro[_i + 1, _j - 1].Estado == 4)
                        && tabuleiro[_i + 2, _j - 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Baixo esquerda - Jogador 2 (dama) - Come peça na extremidade inferior
                else if (_i + 2 == 7
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i + 1, _j - 1].Estado == 1 || tabuleiro[_i + 1, _j - 1].Estado == 3)
                        && tabuleiro[_i + 2, _j - 2].Estado == 0
                        && _estado == 4)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i + 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i + 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                /*
                FIM BAIXO ESQUERDA
                */
            }
            if (_estado == 2 || _estado == 3 || _estado == 4)
            {
                /*
                INÍCIO CIMA DIREITA
                */
                //Movimento: Cima direita - Jogador 1 (dama) e Jogador 2 (peão/dama) - Simples
                if (_i - 1 >= 1
                   && _j + 1 <= 7
                   && tabuleiro[_i - 1, _j + 1].Estado == 0)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 2 (peão) - Transforma dama
                else if (_i - 1 == 0
                   && _j + 1 <= 7
                   && tabuleiro[_i - 1, _j + 1].Estado == 0
                   && _estado == 2)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 1 (dama) e Jogador 2 (dama) - Não transforma dama
                else if (_i - 1 == 0
                   && _j + 1 <= 7
                   && tabuleiro[_i - 1, _j + 1].Estado == 0
                   && (_estado == 3 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j + 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 2 (peão/dama) - Come peça
                else if (_i - 2 >= 1
                        && _j + 2 <= 7
                        && (tabuleiro[_i - 1, _j + 1].Estado == 1 || tabuleiro[_i - 1, _j + 1].Estado == 3)
                        && tabuleiro[_i - 2, _j + 2].Estado == 0
                        && (_estado == 2 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 1 (dama) - Come peça
                else if (_i - 2 >= 1
                        && _j + 2 <= 7
                        && (tabuleiro[_i - 1, _j + 1].Estado == 2 || tabuleiro[_i - 1, _j + 1].Estado == 4)
                        && tabuleiro[_i - 2, _j + 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 2 (peão) - Come peça na extremidade superior e transforma dama
                else if (_i - 2 == 0
                        && _j + 2 <= 7
                        && (tabuleiro[_i - 1, _j + 1].Estado == 1 || tabuleiro[_i - 1, _j + 1].Estado == 3)
                        && tabuleiro[_i - 2, _j + 2].Estado == 0
                        && _estado == 2)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 2 (dama) - Come peça na extremidade superior
                else if (_i - 2 == 0
                        && _j + 2 <= 7
                        && (tabuleiro[_i - 1, _j + 1].Estado == 1 || tabuleiro[_i - 1, _j + 1].Estado == 3)
                        && tabuleiro[_i - 2, _j + 2].Estado == 0
                        && _estado == 4)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima direita - Jogador 1 (dama) - Come peça na extremidade superior
                else if (_i - 2 == 0
                        && _j + 2 <= 7
                        && (tabuleiro[_i - 1, _j + 1].Estado == 2 || tabuleiro[_i - 1, _j + 1].Estado == 4)
                        && tabuleiro[_i - 2, _j + 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j + 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j + 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                /*
                FIM CIMA DIREITA
                */

                /*
                INÍCIO CIMA ESQUERDA
                */
                //Movimento: Cima esquerda - Jogador 1 (dama) e Jogador 2 (peão/dama) - Simples
                if (_i - 1 >= 1
                   && _j - 1 <= 7
                   && _j - 1 >= 0
                   && tabuleiro[_i - 1, _j - 1].Estado == 0)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j - 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 2 (peão) - Transforma dama
                else if (_i - 1 == 0
                   && _j - 1 <= 7
                   && _j - 1 >= 0
                   && tabuleiro[_i - 1, _j - 1].Estado == 0
                   && _estado == 2)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j - 1)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 1 (dama) e Jogador 2 (dama) - Não transforma dama
                else if (_i - 1 == 0
                   && _j - 1 <= 7
                   && _j - 1 >= 0
                   && tabuleiro[_i - 1, _j - 1].Estado == 0
                   && (_estado == 3 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 1)
                                                             ,
                        y = (_j - 1)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = null
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 2 (peão/dama) - Come peça
                else if (_i - 2 >= 1
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i - 1, _j - 1].Estado == 1 || tabuleiro[_i - 1, _j - 1].Estado == 3)
                        && tabuleiro[_i - 2, _j - 2].Estado == 0
                        && (_estado == 2 || _estado == 4))
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 1 (dama) - Come peça
                else if (_i - 2 >= 1
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i - 1, _j - 1].Estado == 2 || tabuleiro[_i - 1, _j - 1].Estado == 4)
                        && tabuleiro[_i - 2, _j - 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 2 (peão) - Come peça na extremidade superior e transforma dama
                else if (_i - 2 == 0
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i - 1, _j - 1].Estado == 1 || tabuleiro[_i - 1, _j - 1].Estado == 3)
                        && tabuleiro[_i - 2, _j - 2].Estado == 0
                        && _estado == 2)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = (_estado + 2)
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 2 (dama) - Come peça na extremidade superior
                else if (_i - 2 == 0
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i - 1, _j - 1].Estado == 1 || tabuleiro[_i - 1, _j - 1].Estado == 3)
                        && tabuleiro[_i - 2, _j - 2].Estado == 0
                        && _estado == 4)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                //Movimento: Cima esquerda - Jogador 1 (dama) - Come peça na extremidade superior
                else if (_i - 2 == 0
                        && _j - 2 <= 7
                        && _j - 2 >= 0
                        && (tabuleiro[_i - 1, _j - 1].Estado == 2 || tabuleiro[_i - 1, _j - 1].Estado == 4)
                        && tabuleiro[_i - 2, _j - 2].Estado == 0
                        && _estado == 3)
                {
                    _coordenadasValidadas.Add(new Coordenada
                    {
                        x = (_i - 2)
                                                             ,
                        y = (_j - 2)
                                                             ,
                        peca_jogador = _estado
                                                             ,
                        come_peca = new int[] { (_i - 1), (_j - 1) }
                                                             ,
                        coord_pai = new int[] { _i, _j }
                                                             ,
                        ID = _ID
                    });
                }
                /*
                FIM CIMA ESQUERDA
                */
            }

            /*
            FIM DA VALIDAÇÃO DE MOVIMENTOS
            */
        }

        public void SinalizaMovimentos(List<Coordenada> movimentoValido)
        {
            if (movimentoValido.Where(cd => cd.come_peca != null).Count() > 0)
                movimentoValido = movimentoValido.Where(cd => cd.come_peca != null).ToList();

            foreach (Coordenada xy in movimentoValido)
            {
                if (xy.ID == 1)
                {
                    tabuleiro[xy.x, xy.y].Imagem.BackColor = ColorTranslator.FromHtml("#00FFFF");
                }
                else
                {
                    tabuleiro[xy.x, xy.y].Imagem.BackColor = ColorTranslator.FromHtml("#C83A3A");
                }
            }
        }
        #endregion

        #region MÉTODOS DO MODO SINGLE PLAYER
        //Verifica quais peças do PC podem executar algum movimento
        public void PcVerificaPecasEasy()
        {
            //Encontra as peças do PC
            int maisMovimentos = 0;
            var casaDestino = new CasaTabuleiro();
            List<Coordenada> listaCoordPC = new List<Coordenada>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tabuleiro[i, j].Estado == 2 || tabuleiro[i, j].Estado == 4)
                    {
                        CasaTabuleiro pecaPC = tabuleiro[i, j];
                        ValidaMovimentos(pecaPC);
                        if (_coordenadasValidadas.Count > maisMovimentos)
                        {
                            maisMovimentos = _coordenadasValidadas.Count();
                            if (_coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null).Count() > 0)
                            {
                                var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null).FirstOrDefault();
                                listaCoordPC.Add(coordenadaDestinoMov);
                                casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];

                                ValidaMovimentos(casaDestino);
                            }
                            else if (_coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca == null).Count() > 0)
                            {
                                var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca == null).FirstOrDefault();
                                listaCoordPC.Add(coordenadaDestinoMov);
                                casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];
                            }
                        }
                    }
                }
            }
            PecaPula(casaDestino, listaCoordPC);

            if (_coordenadasValidadas.Where(coordPC => coordPC.come_peca != null
                                            && coordPC.coord_pai[0] == casaDestino.i
                                            && coordPC.coord_pai[1] == casaDestino.j).Count() > 0)
            {
                listaCoordPC = new List<Coordenada>();
                var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null
                                                                       && coordenadapc.coord_pai[0] == casaDestino.i
                                                                       && coordenadapc.coord_pai[1] == casaDestino.j).FirstOrDefault();
                listaCoordPC.Add(coordenadaDestinoMov);
                casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];

                Application.DoEvents();
                Thread.Sleep(1000);

                PecaPula(casaDestino, listaCoordPC);
            }

        }

        public void PcVerificaPecasNormal()
        {
            //Encontra as peças do PC
            bool segundaTomada = false;
            var casaDestino = new CasaTabuleiro();
            CasaTabuleiro pecaPC;
            List<Coordenada> listaCoordPC = new List<Coordenada>();
            List<Coordenada> listaTomadaPC = new List<Coordenada>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tabuleiro[i, j].Estado == 2 || tabuleiro[i, j].Estado == 4)
                    {
                        pecaPC = new CasaTabuleiro();
                        pecaPC = tabuleiro[i, j];
                        ValidaMovimentos(pecaPC);
                        if (_coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null).Count() > 0)
                        {
                            var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null).First();
                            listaCoordPC.Add(coordenadaDestinoMov);
                            listaTomadaPC.Add(coordenadaDestinoMov);
                            casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];

                            //ValidaMovimentos(casaDestino);
                        }
                        else if (listaTomadaPC.Where(cdt => cdt.come_peca != null).Count() == 0
                            && _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca == null).Count() > 0)
                        {
                            var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca == null).First();
                            listaCoordPC.Add(coordenadaDestinoMov);
                            casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];
                        }
                    }
                }
            }
            if (listaTomadaPC.Where(cdt => cdt.come_peca != null).Count() > 0)
            {
                PecaPula(casaDestino, listaTomadaPC);
                segundaTomada = true;
            }
            else
            {
                PecaPula(casaDestino, listaCoordPC);
            }

            ValidaMovimentos(casaDestino);
            if (segundaTomada == true && _coordenadasValidadas.Where(coordPC => coordPC.come_peca != null
                                            && coordPC.coord_pai[0] == casaDestino.i
                                            && coordPC.coord_pai[1] == casaDestino.j).Count() > 0)
            {
                listaCoordPC = new List<Coordenada>();
                var coordenadaDestinoMov = _coordenadasValidadas.Where(coordenadapc => coordenadapc.come_peca != null
                                                                       && coordenadapc.coord_pai[0] == casaDestino.i
                                                                       && coordenadapc.coord_pai[1] == casaDestino.j).FirstOrDefault();
                listaCoordPC.Add(coordenadaDestinoMov);
                casaDestino = tabuleiro[coordenadaDestinoMov.x, coordenadaDestinoMov.y];

                Application.DoEvents();
                Thread.Sleep(1000);

                PecaPula(casaDestino, listaCoordPC);

                ValidaMovimentos(casaDestino);
            }

        }


        #endregion

        #region MÉTODOS DE RECUPERAÇÃO DE JOGO SALVO

        public void SalvaJogo ()
        {
            int[,] posicoesPecas = new int[8,8];

            for (int i = 0; i <8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    posicoesPecas[i, j] = tabuleiro[i, j].Estado;
                }
            }

            var dadosJson = new
            {
                turnoJogador = jogadorTurno,
                tempoJogoGeral = lbMinutos.Text + lbDoisPontos.Text + lbSegundos.Text,
                tempoJogador1 = lbMinutos1.Text + lbDoisPontos1.Text + lbSegundos1.Text,
                tempoJogador2 = lbMinutos2.Text + lbDoisPontos2.Text + lbSegundos2.Text,
                tabuleiro = posicoesPecas
            };

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dadosJson);

            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\"+"JogoDamasSaveGame.log",jsonString);
        }

        public bool VerificaContinuacaoJogo()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "JogoDamasSaveGame.log") && MessageBox.Show("Existe um jogo que ainda não foi finalizado.\r\nDeseja continuar o jogo anterior?","Jogo não terminado!",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var leitor = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\" + "JogoDamasSaveGame.log"))
                    {
                        var jsonObjeto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(leitor.ReadToEnd());
                        lbMinutos.Text = ((string)jsonObjeto.tempoJogoGeral).Split(':')[0];
                        lbSegundos.Text = ((string)jsonObjeto.tempoJogoGeral).Split(':')[1];
                        lbMinutos1.Text = ((string)jsonObjeto.tempoJogador1).Split(':')[0];
                        lbSegundos1.Text = ((string)jsonObjeto.tempoJogador1).Split(':')[1];
                        lbMinutos2.Text = ((string)jsonObjeto.tempoJogador2).Split(':')[0];
                        lbMinutos2.Text = ((string)jsonObjeto.tempoJogador2).Split(':')[1];
                        jogadorTurno = (int)jsonObjeto.turnoJogador;

                        int[,] estadoTabuleiroJson = new int[8,8];

                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                estadoTabuleiroJson[i, j] = (int)(jsonObjeto.tabuleiro[i])[j];
                            }
                        }
                        MontaTabuleiro(estadoTabuleiroJson);
                    }
                    return true;
                }
                catch { return false;  }
            }
            return false;
        }

        #endregion

        private void JogoDama_JogoDamaClosing_2(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show(this, "Tem certeza que deseja fechar este aplicativo?", "Fechando...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    tmGame.Stop();
                    SalvaJogo();
                    //e.Cancel = false;
                    Application.Restart();
                    //Application.Exit();
                }
            }
        }
    }
}
    public class Coordenada
    {
        public int x { get; set; }
        public int y { get; set; }
        public int peca_jogador { get; set; }
        public int[] come_peca { get; set; }
        public int[] coord_pai { get; set; }
        public int ID { get; set; }
    }

