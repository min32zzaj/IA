using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avioane
{
    internal class Airplane
    {
        public int HeadRow { get; set; }
        public int HeadColumn { get; set; }
        public string Orientation { get; set; }

        public List<(int Row, int Column)> GetOccupiedPositions()
        {
            var positions = new List<(int Row, int Column)>
            {
                (HeadRow, HeadColumn)
            };

            switch (Orientation)
            {
                case "up":
                    positions.Add((HeadRow + 1, HeadColumn)); 
                    positions.Add((HeadRow + 2, HeadColumn));
                    positions.Add((HeadRow + 1, HeadColumn-1));
                    positions.Add((HeadRow + 1, HeadColumn + 1));
                    break;
                case "down":
                    positions.Add((HeadRow - 1, HeadColumn));
                    positions.Add((HeadRow - 2, HeadColumn));
                    positions.Add((HeadRow - 1, HeadColumn - 1));
                    positions.Add((HeadRow - 1, HeadColumn + 1));
                    break;
                case "left":
                    positions.Add((HeadRow, HeadColumn + 1));
                    positions.Add((HeadRow, HeadColumn + 2));
                    positions.Add((HeadRow - 1, HeadColumn + 1));
                    positions.Add((HeadRow + 1, HeadColumn + 1));
                    break;
                case "right":
                    positions.Add((HeadRow, HeadColumn - 1));
                    positions.Add((HeadRow, HeadColumn - 2));
                    positions.Add((HeadRow - 1, HeadColumn - 1));
                    positions.Add((HeadRow + 1, HeadColumn - 1));
                    break;
            }

        return positions;
        }

        public bool IsValid(int gridSize)
        {
            var positions = GetOccupiedPositions();
            foreach (var (row, col) in positions)
            {
                if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
