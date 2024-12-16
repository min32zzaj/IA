using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avioane
{
    internal class Game
    {
        public Player HumanPlayer { get; private set; }
        public Player ComputerPlayer { get; private set; }

        public Game()
        {
            HumanPlayer = new Player();
            ComputerPlayer = new Player();
        }
    }
}
