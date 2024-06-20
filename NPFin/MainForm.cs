using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPFin
{
    public partial class MainForm : Form
    {
        private static UdpClient _client;
        private static IPEndPoint _serverEndPoint;
        private int _round;
        private int _winRound;
        private int _loseRound;

        public MainForm()
        {
            InitializeComponent();

            _client = new UdpClient();
            _serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53333);
            _round = 0;
            _winRound = 0;
            _loseRound = 0;
        }
        private void HandleGameEnded()
        {
            panelChoice.Visible = false;
            panelRound.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            pictureBox4.Visible = false;
            pb_MyC.Visible = false;
            pb_OppC.Visible = false;

            Label result = new Label();
            result.AutoSize = true;
            result.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            result.Location = new System.Drawing.Point(503, 248);
            result.Size = new System.Drawing.Size(138, 51);

            if (_winRound > _loseRound)
            {
                result.Text = "You Win!";
                result.ForeColor = Color.Green;
            }
            else if (_winRound == _loseRound)
            {
                result.Text = "Draw!";
                result.ForeColor = Color.Gray;
            }  
            else
            {
                result.Text = "You Lose!";
                result.ForeColor = Color.Red;
            }
                

            Controls.Add(result);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _client.Close();
        }

        private void p_btn_Paper_Click(object sender, EventArgs e)
        {
            pb_MyC.Image = global::NPFin.Properties.Resources.paper_removebg_preview;
            pb_MyC.Tag = "Paper";
            SendChoice("Paper");
            DisableChoiceBtn();
        }

        private void p_btn_Scissors_Click(object sender, EventArgs e)
        {
            pb_MyC.Image = global::NPFin.Properties.Resources.scissors_removebg_preview;
            pb_MyC.Tag = "Scissors";
            SendChoice("Scissors");
            DisableChoiceBtn();
        }

        private void p_btn_Stone_Click(object sender, EventArgs e)
        {
            pb_MyC.Image = global::NPFin.Properties.Resources.stone_removebg_preview;
            pb_MyC.Tag = "Stone";
            SendChoice("Stone");
            DisableChoiceBtn();
        }
        private void SendChoice(string choice)
        {
            byte[] choiceBytes = Encoding.UTF8.GetBytes(choice);
            _client.Send(choiceBytes, choiceBytes.Length, _serverEndPoint);
            Task.Run(() => ReceiveResult());
        }

        private void ReceiveResult()
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = _client.Receive(ref serverEndPoint);
                string result = Encoding.UTF8.GetString(buffer);
                Invoke(new Action(() => { DisplayResult(result); }));
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private void DisplayResult(string result)
        {
            _round++;

            switch (result)
            {
                case "Stone":
                    pb_OppC.Image = global::NPFin.Properties.Resources.stone_removebg_preview;
                    pb_OppC.Tag = "Stone";
                    break;
                case "Scissors":
                    pb_OppC.Image = global::NPFin.Properties.Resources.scissors_removebg_preview;
                    pb_OppC.Tag = "Scissors";
                    break;
                case "Paper":
                    pb_OppC.Image = global::NPFin.Properties.Resources.paper_removebg_preview;
                    pb_OppC.Tag = "Paper";
                    break;
                default:
                    pb_OppC.Image = global::NPFin.Properties.Resources.stone_removebg_preview;
                    pb_OppC.Tag = "Stone";
                    break;
            }

            string myChoice = pb_MyC.Tag.ToString();
            string opponentChoice = pb_OppC.Tag.ToString();
            Label currentLbl = GetCurrentRound();

            if (myChoice == opponentChoice)
            {
                currentLbl.Text = "Draw";
                currentLbl.ForeColor = Color.Gray;
            }
            else if ((myChoice == "Stone" && opponentChoice == "Scissors") ||
                     (myChoice == "Scissors" && opponentChoice == "Paper") ||
                     (myChoice == "Paper" && opponentChoice == "Stone"))
            {
                currentLbl.Text = "Win";
                currentLbl.ForeColor = Color.Green;
                _winRound++;
            }
            else
            {
                currentLbl.Text = "Lose";
                currentLbl.ForeColor = Color.Red;
                _loseRound++;
            }
            currentLbl.Visible = true;

            pb_OppC.Image = null;

            EnableChoiceBtn();

            if (_round == 5)
            {
                Invoke(new Action(() => { HandleGameEnded(); }));
                _client?.Dispose();
            }
        }

        private Label GetCurrentRound()
        {
            foreach (Label item in panelRound.Controls)
            {
                if (item is Label lbl && int.Parse(lbl.Tag.ToString()) == _round)
                {
                    return lbl;
                }
            }
            return new Label();
        }

        private void DisableChoiceBtn()
        {
            p_btn_Paper.Click -= p_btn_Paper_Click;
            btn_Paper.Click -= p_btn_Paper_Click;
            pictureBoxPaper.Click -= p_btn_Paper_Click;

            p_btn_Scissors.Click -= p_btn_Scissors_Click;
            btn_scissors.Click -= p_btn_Scissors_Click;
            pictureBoxScissors.Click -= p_btn_Scissors_Click;

            p_btn_Stone.Click -= p_btn_Stone_Click;
            btn_Stone.Click -= p_btn_Stone_Click;
            pictureBoxStone.Click -= p_btn_Stone_Click;
        }

        private void EnableChoiceBtn()
        {
            p_btn_Paper.Click += p_btn_Paper_Click;
            btn_Paper.Click += p_btn_Paper_Click;
            pictureBoxPaper.Click += p_btn_Paper_Click;

            p_btn_Scissors.Click += p_btn_Scissors_Click;
            btn_scissors.Click += p_btn_Scissors_Click;
            pictureBoxScissors.Click += p_btn_Scissors_Click;

            p_btn_Stone.Click += p_btn_Stone_Click;
            btn_Stone.Click += p_btn_Stone_Click;
            pictureBoxStone.Click += p_btn_Stone_Click;
        }
    }
}
