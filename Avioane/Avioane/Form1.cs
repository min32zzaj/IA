using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avioane
{
    public partial class Form1 : Form
    {
        private Game game;
        private int planes = 0;
        // grid pentru calculator (AI), ce "stie" calculatorul despre tabla jucatorului
        private int[,] aiGrid = new int[Grid.GridSize, Grid.GridSize];
        public Form1()
        {
            InitializeComponent();
            game = new Game();
            InitializeGameGrid();

            //resetam grila AI la -1 (necunoscut)
            for (int r = 0; r < Grid.GridSize; r++)
            {
                for (int c = 0; c < Grid.GridSize; c++)
                {
                    aiGrid[r, c] = -1;
                }
            }

            PlaceComputerPlanes();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeGameGrid()
        {
            foreach (var control in this.Controls)
            {
                if (control is Button button && button.Name.Contains("_c"))
                {
                    button.Enabled = false;
                }
            }
        }

        private void PlaceComputerPlanes()
        {
            game.ComputerPlayer.Airplanes.Clear();
            for (int r = 0; r < Grid.GridSize; r++)
            {
                for (int c = 0; c < Grid.GridSize; c++)
                {
                    game.ComputerPlayer.PlayerGrid.Cells[r, c] = 0;
                }
            }

            Random rand = new Random();
            string[] orientations = { "up", "down", "left", "right" };

            int planesNeeded = 3;
            int planesPlaced = 0;

            while (planesPlaced < planesNeeded)
            {
                int row = rand.Next(0, Grid.GridSize);
                int col = rand.Next(0, Grid.GridSize);
                string orientation = orientations[rand.Next(orientations.Length)];

                var airplane = new Airplane
                {
                    HeadRow = row,
                    HeadColumn = col,
                    Orientation = orientation
                };

                if (game.ComputerPlayer.AddAirplane(airplane))
                {
                    planesPlaced++;
                }
            }
        }


        private void ResetPlayerGrid()
        {
            for (int row = 0; row < Grid.GridSize; row++)
            {
                for (int col = 0; col < Grid.GridSize; col++)
                {
                    game.HumanPlayer.PlayerGrid.Cells[row, col] = 0;
                    game.ComputerPlayer.PlayerGrid.Cells[row, col] = 0;
                }
            }
        }


        private void UpdateGridDisplay()
        {
            foreach (var control in this.Controls)
            {
                if (control is Button button && button.Name.Contains("_p"))
                {
                    string position = button.Name.Replace("bt_", "").Replace("_p", "");
                    int col = int.Parse(position.Substring(0, position.Length - 1)) - 1;
                    int row = position[position.Length - 1] - 'a';

                    if (game.HumanPlayer.PlayerGrid.Cells[row, col] == 2)
                    {
                        button.BackColor = Color.Green;
                    }
                    else if (game.HumanPlayer.PlayerGrid.Cells[row, col] == 1)
                    {
                        button.BackColor = Color.Blue;
                    }
                    else if (game.HumanPlayer.PlayerGrid.Cells[row, col] == 3)
                    {
                        button.BackColor = Color.Red;
                        button.Enabled = false;
                    }
                    else if (game.HumanPlayer.PlayerGrid.Cells[row, col] == 4)
                    {
                        button.BackColor = Color.Gray;
                        button.Enabled = false;
                    }
                    else
                    {
                        button.BackColor = SystemColors.Control;
                    }
                }else if(control is Button button1 && button1.Name.Contains("_c"))
                {
                    string position = button1.Name.Replace("bt_", "").Replace("_c", "");
                    int col = int.Parse(position.Substring(0, position.Length - 1)) - 1;
                    int row = position[position.Length - 1] - 'a';

                    if(game.ComputerPlayer.PlayerGrid.Cells[row, col] == 3)
                    {
                        button1.BackColor = Color.Red;
                        button1.Enabled = false;
                    }
                }
            }
        }


        private void PlacePlane(Button button)
        {
            if (planes >= 3)
                return;

            string position = button.Name.Replace("bt_", "").Replace("_p", "");
            int col = int.Parse(position.Substring(0, position.Length - 1)) - 1;
            int row = position[position.Length - 1] - 'a';

            string orientation = cb_orientation.SelectedItem.ToString();

            //MessageBox.Show($"col = {col}, row={row}, orientation={orientation}");

            var airplane = new Airplane
            {
                HeadRow = row,
                HeadColumn = col,
                Orientation = orientation
            };

            if (!airplane.IsValid(6))
            {
                MessageBox.Show("Avionul nu poate fi plasat! Poziția iese în afara grilei.");
                return;
            }

            if (game.HumanPlayer.AddAirplane(airplane))
            {
                //MessageBox.Show("Avion plasat cu succes!");
                planes++;
                UpdateGridDisplay();
            }
            else
            {
                MessageBox.Show("Locație invalidă! Pozițiile se suprapun.");
            }
        }

        
        private void PlayerAttack(Button button)
        {
            if (button == null)
                return;

            string position = button.Name.Replace("bt_", "").Replace("_c", "");
            int col = int.Parse(position.Substring(0, position.Length - 1)) - 1;
            int row = position[position.Length - 1] - 'a';

            if (game.ComputerPlayer.PlayerGrid.Cells[row, col] == 1)
            {
                button.BackColor = Color.Blue;
                //MessageBox.Show("Hit!");
            }
            else if (game.ComputerPlayer.PlayerGrid.Cells[row, col] == 2)
            {
                button.BackColor = Color.Green;
                HeadShot(row, col, game.ComputerPlayer);
                game.HumanPlayer.score++;
                //MessageBox.Show("Head Shot!");
                
            }
            else
            {
                button.BackColor = Color.Gray;
                //MessageBox.Show("Miss!");
            }
            button.Enabled = false;

            if(game.HumanPlayer.score==3)
            {
                MessageBox.Show("Felicitări, ai câștigat jocul!", "Victorie!");
                EndGame();
                return;
            }
            ComputerAttack();
        }

        private void HeadShot(int row, int col, Player player)
        {
            foreach (var plane in player.Airplanes)
            {
                if(plane.HeadRow==row && plane.HeadColumn == col)
                {
                    var positions=plane.GetOccupiedPositions();
                    foreach (var position in positions)
                    {
                        int posRow = position.Item1;
                        int posCol = position.Item2;
                        player.PlayerGrid.Cells[posRow, posCol] = 3;
                    }
                }
            }
            UpdateGridDisplay();
        }

       
        private void ComputerAttack()
        {
            // MCTS pe baza aiGrid, nu a gridului real
            MonteCarlo mcts = new MonteCarlo(aiGrid);
            int simulations = 1000; // numarul de simulari
            (int row, int col) = mcts.GetBestMove(simulations);

            if (row == -1 || col == -1) return;

            // verific in grila reala a jucatorului
            int realValue = game.HumanPlayer.PlayerGrid.Cells[row, col];

            // in functie de realValue, marchez in aiGrid
            if (realValue == 1)
            {
                // a lovit corp de avion deci in aiGrid pun 3 (hit)
                aiGrid[row, col] = 3;
                // in grila reala tot hit
                game.HumanPlayer.PlayerGrid.Cells[row, col] = 3;
            }
            else if (realValue == 2)
            {
                aiGrid[row, col] = 3;
                game.HumanPlayer.PlayerGrid.Cells[row, col] = 3;

                // headshot in grila reala
                HeadShot(row, col, game.HumanPlayer);

                // adaugam si in aiGrid toate celulele doborate ale avionului prin headshot
                foreach (var plane in game.HumanPlayer.Airplanes)
                {
                    if (plane.HeadRow == row && plane.HeadColumn == col)
                    {
                        var positions = plane.GetOccupiedPositions();
                        foreach (var (posRow, posCol) in positions)
                        {
                            aiGrid[posRow, posCol] = 3;
                        }
                    }
                }
                // creste scorul
                game.ComputerPlayer.score++;
            }

            else
            {
                aiGrid[row, col] = 4;
                game.HumanPlayer.PlayerGrid.Cells[row, col] = 4;
            }

            UpdateGridDisplay();

            if (game.ComputerPlayer.score == 3)
            {
                MessageBox.Show("Ai pierdut jocul!", "Eșec");
                EndGame();
            }
        }



        private void EndGame()
        {
            foreach (var control in this.Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = false;
                }
            }
            bt_reset.Enabled = true;
        }

      
        private void bt_reset_Click(object sender, EventArgs e)
        {
            ResetPlayerGrid();
            PlaceComputerPlanes();
            planes = 0;
            game.HumanPlayer.score = 0;
            game.ComputerPlayer.score = 0;

            // reinitializare aiGrid
            for (int r = 0; r < Grid.GridSize; r++)
            {
                for (int c = 0; c < Grid.GridSize; c++)
                {
                    aiGrid[r, c] = -1;
                }
            }

            foreach (var control in this.Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = true;
                    button.BackColor = SystemColors.Control;
                }
            }
        }


        private void bt_start_Click(object sender, EventArgs e)
        {
            if (planes != 3)
            {
                MessageBox.Show("Plasează toate avioanele înainte de a începe jocul.", "Eroare");
                return;
            }
            foreach (var control in this.Controls)
            {
                if (control is Button button && button.Name.Contains("_c"))
                {
                    button.Enabled = true;
                }
                else if (control is Button playerButton && playerButton.Name.Contains("_p"))
                {
                    playerButton.Enabled = false;
                }
            }

            bt_start.Enabled = false;
            bt_reset.Enabled = false;
        }


        private void bt_1a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6a_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6b_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6c_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6d_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6e_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_2f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_3f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_4f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_5f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_6f_c_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            PlayerAttack(clickedButton);
        }

        private void bt_1a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);

        }

        private void bt_2a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6a_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_1b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_2b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6b_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_1c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_2c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6c_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_1d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_2d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6d_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_1e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_2e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6e_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_1f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_2f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_3f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_4f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_5f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_6f_p_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            PlacePlane(clickedButton);
        }

        private void bt_help_Click(object sender, EventArgs e)
        {
            string helpMessage =
                "Proiect Avioane - Inteligenta Artificiala, implementare MCTS anul 2024-2025\n" +
                "Autori: Balan Iulia, Adam Iasmina Felicia\n\n" +
                "Initial, player-ul va trebui sa pozitioneze pe tabla sa 3 avioane, orientate cum doreste. " +
                "Daca doreste resetarea tablei, va apasa pe 'Reset'. " +
                "Pentru a incepe jocul, va apasa pe butonul 'Start'.\n\n" +
                "Scopul este sa gaseasca pe tabla Calculatorului 3 avioane (in forma de +). " +
                "Daca nimereste capul unui avion (headshot), tot avionul se va prabusi. " +
                "Primul jucator care doboara toate cele 3 avioane ale oponentului castiga.";

            MessageBox.Show(helpMessage, "Help");
        }

    }
}
