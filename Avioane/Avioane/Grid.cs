using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avioane
{
    internal class Grid
    {
        public const int GridSize = 6;
        public int[,] Cells { get; private set; }
        public Grid()
        {
            Cells = new int[GridSize+1, GridSize+1];
        }

        public bool CanPlaceAirplane(Airplane airplane)
        {
            if (!airplane.IsValid(GridSize))
            {
                return false;
            }
            var positions = airplane.GetOccupiedPositions();
            foreach (var (row, col) in positions)
            {
                if (row < 0 || row >= GridSize || col < 0 || col >= GridSize || Cells[row, col] == 1 || Cells[row, col] == 2)
                {
                    return false;
                }
            }
            return true;
        }

        public void PlaceAirplane(Airplane airplane)
        {
            var positions = airplane.GetOccupiedPositions();

            if (positions.Count > 0)
            {
                var (headRow, headCol) = positions[0];
                Cells[headRow, headCol] = 2;

                for (int i = 1; i < positions.Count; i++)
                {
                    var (row, col) = positions[i];
                    Cells[row, col] = 1;
                }
            }
        }
    }
}
