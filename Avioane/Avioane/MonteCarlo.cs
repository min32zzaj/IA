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
                .Where(move => !node.Children.Any(child => child.Row == move.Item1 && child.Col == move.Item2))
                .ToList();

            if (unexploredMoves.Count > 0)
            {
                var randomMove = unexploredMoves[rand.Next(unexploredMoves.Count)];
                var newNode = new Node(randomMove.Item1, randomMove.Item2, node);
                node.Children.Add(newNode);
                return newNode;
            }

            return node; // toate mutarile au fost explorate
        }

        // Faza 3: Simularea
        private int Simulation(Node node)
        {
            var simulationGrid = (int[,])grid.Cells.Clone();
            var row = node.Row;
            var col = node.Col;

            for (int i = 0; i < 10; i++)
            {
                if (simulationGrid[row, col] == 2)
                {
                    return 1;
                }
                else if (simulationGrid[row, col] == 1)
                {
                    simulationGrid[row, col] = 3;
                    var nextMove = GetRandomMove(simulationGrid);
                    if (nextMove == (-1, -1))
                        break;
                    row = nextMove.Item1;
                    col = nextMove.Item2;
                }
                else if (simulationGrid[row, col] == 0)
                {
                    simulationGrid[row, col] = 4;
                }
                else
                {
                    break;
                }
            }

            return 0;
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

        // Mutare aleatorie
        private (int, int) GetRandomMove(int[,] simulationGrid)
        {
            var availableMoves = new List<(int, int)>();

            for (int row = 0; row < Grid.GridSize; row++)
            {
                for (int col = 0; col < Grid.GridSize; col++)
                {
                    if (simulationGrid[row, col] != 3 && simulationGrid[row, col] != 4)
                        availableMoves.Add((row, col));
                }
            }

            if (availableMoves.Count == 0)
                return (-1, -1);

            return availableMoves[rand.Next(availableMoves.Count)];
        }
    }
}