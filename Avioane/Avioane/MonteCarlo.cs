using System;
using System.Collections.Generic;
using System.Linq;

namespace Avioane
{
    internal class MonteCarlo
    {
        private static Random rand = new Random();

        // reprezentarea nodurilor din arbore
        public class Node
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public int Wins { get; set; }
            public int Simulations { get; set; }
            public List<Node> Children { get; set; }
            public Node Parent { get; set; }

            public Node(int row, int col, Node parent = null)
            {
                Row = row;
                Col = col;
                Wins = 0;
                Simulations = 0;
                Children = new List<Node>();
                Parent = parent;
            }

            // calcul UCB1 pentru selectie
            public double UCB1(int totalSimulations)
            {
                if (Simulations == 0)
                    return double.MaxValue; // daca nodul curent nu a fost explorat, are prioritate maxima
                return (double)Wins / Simulations + Math.Sqrt(2 * Math.Log(totalSimulations) / Simulations);
            }
        }

        private Node root;
        private Grid grid;

        public MonteCarlo(Grid grid)
        {
            this.grid = grid;
            root = new Node(-1, -1); // root este toata tabla
        }

        // Metoda pentru alegerea celei mai bune mutari
        public (int, int) GetBestMove(int simulations)
        {
            for (int i = 0; i < simulations; i++)
            {
                Node selectedNode = Selection(root);
                Node expandedNode = Expansion(selectedNode);
                int simulationResult = Simulation(expandedNode);
                Backpropagation(expandedNode, simulationResult);
            }

            // alege mutarea cu cele mai multe simulari
            Node bestMove = root.Children.OrderByDescending(c => c.Simulations).FirstOrDefault();
            if (bestMove == null)
                return (-1, -1);

            return (bestMove.Row, bestMove.Col);
        }

        // Faza 1: Selectia
        private Node Selection(Node node)
        {
            while (node.Children.Count > 0)
            {
                node = node.Children.OrderByDescending(c => c.UCB1(node.Simulations)).First();
            }
            return node;
        }

        // Faza 2: Expandarea
        private Node Expansion(Node node)
        {
            var availableMoves = GetAvailableMoves();
            var unexploredMoves = availableMoves
                .Where(move => !node.Children.Any(child => child.Row == move.Item1 && child.Col == move.Item2)).ToList();

            if (unexploredMoves.Count > 0)
            {
                // daca avem lovituri, preferam mutari vecine cu loviturile
                var bestMoves = GetMovesNearHits(unexploredMoves);

                // daca bestMoves e gol, alegem mutare random din unexplored
                var chosenMove = bestMoves.Count > 0
                    ? bestMoves[rand.Next(bestMoves.Count)]
                    : unexploredMoves[rand.Next(unexploredMoves.Count)];

                var newNode = new Node(chosenMove.Item1, chosenMove.Item2, node);
                node.Children.Add(newNode);
                return newNode;
            }

            return node; // daca toate mutarile au fost explorate, ramanem pe nodul curent
        }

        // Faza 3: Simularea
        private int Simulation(Node node)
        {
            // clonam starea grilei pt a nu strica datele reale
            var simulationGrid = (int[,])grid.Cells.Clone();

            var row = node.Row;
            var col = node.Col;

            if (row < 0 || col < 0)
                return 0;

            // incercam pana la max 10 atacuri in lant
            int steps = 0;
            bool foundHead = false;
            while (steps < 10)
            {
                steps++;
                if (row < 0 || row >= Grid.GridSize || col < 0 || col >= Grid.GridSize)
                    break;

                // ce era pe celula inainte de atac
                int cellValue = simulationGrid[row, col];

                // cazuri
                // 2 = cap de avion => victorie
                // 1 = parte de avion => il marcam drept lovit (3) si incercam sa continuam atacul in vecinatate
                // 0 = celula goala => ratat (4)
                // 3 = deja lovit => break
                // 4 = deja marcat ratat => break
                if (cellValue == 2)
                {
                    foundHead = true;
                    break;
                }
                else if (cellValue == 1)
                {
                    simulationGrid[row, col] = 3;
                    (row, col) = GetCloseHitNeighbor(simulationGrid, row, col);
                }
                else if (cellValue == 0)
                {
                    simulationGrid[row, col] = 4;
                    break;
                }
                else
                {
                    break;
                }
            }

            // daca am gasit un cap de avion, scor devine 1
            return foundHead ? 1 : 0;
        }

        // Faza 4: Actualizarea (Retro-propagarea)
        private void Backpropagation(Node node, int result)
        {
            while (node != null)
            {
                node.Simulations++;
                if (result == 1) node.Wins++;
                node = node.Parent;
            }
        }

        // Mutarile disponibile pe tabla
        private List<(int, int)> GetAvailableMoves()
        {
            var moves = new List<(int, int)>();
            for (int row = 0; row < Grid.GridSize; row++)
            {
                for (int col = 0; col < Grid.GridSize; col++)
                {
                    if (grid.Cells[row, col] != 3 && grid.Cells[row, col] != 4) // exclude celulele lovite sau ratate
                    {
                        moves.Add((row, col));
                    }
                }
            }
            return moves;
        }


        // Returneaza o lista de mutari langa celulele marcate '1' (parte avion) sau '2' (cap avion)
        private List<(int, int)> GetMovesNearHits(List<(int, int)> candidateMoves)
        {
            // extragem toate celulele de tip 1 sau 2
            var hits = new List<(int, int)>();
            for (int r = 0; r < Grid.GridSize; r++)
            {
                for (int c = 0; c < Grid.GridSize; c++)
                {
                    if (grid.Cells[r, c] == 1 || grid.Cells[r, c] == 2)
                    {
                        hits.Add((r, c));
                    }
                }
            }

            // daca nu exista niciun hit, nu avem ce prioritiza
            if (hits.Count == 0)
                return new List<(int, int)>();

            // salvam toate celulele candidate care sunt (row, col) adiacente vreunui hit
            var nearHits = new List<(int, int)>();
            foreach (var (hr, hc) in hits)
            {
                var neighbors = GetNeighbors(hr, hc);
                nearHits.AddRange(neighbors.Where(n => candidateMoves.Contains(n)));
            }

            // scoatem duplicatele
            nearHits = nearHits.Distinct().ToList();

            return nearHits;
        }

        // Returneaza toate vecinatatile unei celule (sus, jos, stanga, dreapta)
        private List<(int, int)> GetNeighbors(int row, int col)
        {
            var result = new List<(int, int)>();
            if (row - 1 >= 0) result.Add((row - 1, col));
            if (row + 1 < Grid.GridSize) result.Add((row + 1, col));
            if (col - 1 >= 0) result.Add((row, col - 1));
            if (col + 1 < Grid.GridSize) result.Add((row, col + 1));
            return result;
        }

        // Returneaza o celula nelovita sau ratata, vecina ultimei lovituri
        private (int, int) GetCloseHitNeighbor(int[,] simulationGrid, int row, int col)
        {
            var neighbors = GetNeighbors(row, col);
            // alegem un vecin care inca nu e marcat cu 3 (lovit) sau 4 (ratat)
            var candidates = new List<(int, int)>();
            foreach (var (r, c) in neighbors)
            {
                if (simulationGrid[r, c] != 3 && simulationGrid[r, c] != 4)
                {
                    candidates.Add((r, c));
                }
            }

            if (candidates.Count == 0)
                return (-1, -1);

            return candidates[rand.Next(candidates.Count)];
        }
    }
}