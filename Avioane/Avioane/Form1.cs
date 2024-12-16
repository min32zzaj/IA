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
        public Form1()
        {
            InitializeComponent();
            game = new Game();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
                    else
                    {
                        button.BackColor = SystemColors.Control;
                    }
                }
            }
        }


        private void PlacePlane(Button button)
        {
            string position = button.Name.Replace("bt_", "").Replace("_p", "");
            int col = int.Parse(position.Substring(0, position.Length - 1)) - 1;
            int row = position[position.Length - 1] - 'a';

            string orientation = cb_orientation.SelectedItem.ToString();

            MessageBox.Show($"col = {col}, row={row}, orientation={orientation}");

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
                MessageBox.Show("Avion plasat cu succes!");
                UpdateGridDisplay();
            }
            else
            {
                MessageBox.Show("Locație invalidă! Pozițiile se suprapun.");
            }
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
    }
}
