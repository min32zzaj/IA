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

        //starea cunoscuta a AI-ului
        private int[,] knownGrid;
        private const int GridSize = 6;

        public MonteCarlo(int[,] aiGrid)
        {
            knownGrid = (int[,])aiGrid.Clone();
            root = new Node(-1, -1);
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
                (int row, int col) chosenMove;

                if (bestMoves.Count > 0)
                {
                    chosenMove = bestMoves[rand.Next(bestMoves.Count)];
                }
                else
                {
                    chosenMove = unexploredMoves[rand.Next(unexploredMoves.Count)];
                }

                var newNode = new Node(chosenMove.row, chosenMove.col, node);
                node.Children.Add(newNode);
                return newNode;
            }

            return node; // daca toate mutarile au fost explorate, ramanem pe nodul curent
        }


        // Faza 3: Simularea
        private int Simulation(Node node)
        {
            // clonam starea grilei AI
            var simulationGrid = (int[,])knownGrid.Clone();

            int row = node.Row;
            int col = node.Col;

            // daca e -1 (necunoscut), simulam "lovitura" => marcam 3 (hit)
            // simulam pana la max 10 atacuri in lant
            int steps = 0;
            bool foundSomething = false; // victoria e subiectiva aici, nu stim de cap de avion

            while (steps < 10)
            {
                steps++;
                if (row < 0 || row >= GridSize || col < 0 || col >= GridSize)
                    break;

                int cellValue = simulationGrid[row, col];
                // in aiGrid, stim doar: -1 (necunoscut), 3 (lovit), 4 (ratat)

                if (cellValue == -1)
                {
                    // incercam un "hit" fictiv in simulare
                    simulationGrid[row, col] = 3;
                    foundSomething = true; // consideram ca "am lovit ceva"
                    (row, col) = GetCloseNeighbor(simulationGrid, row, col);
                }
                else
                {
                    // daca e deja 3 sau 4, plecam
                    break;
                }
            }

            return foundSomething ? 1 : 0;
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

        private List<(int, int)> GetAvailableMoves()
        {
            var moves = new List<(int, int)>();
            for (int row = 0; row < Grid.GridSize; row++)
            {
                for (int col = 0; col < Grid.GridSize; col++)
                {
                    if (knownGrid[row, col] == -1) // doar necunoscutele sunt posibile
                    {
                        moves.Add((row, col));
                    }
                }
            }
            return moves;
        }

        // Returneaza o lista de mutari langa "lovituri" (3) 
        private List<(int, int)> GetMovesNearHits(List<(int, int)> candidateMoves)
        {
            // extragem toate "hit"-urile
            var hits = new List<(int, int)>();
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    if (knownGrid[r, c] == 3) // deja marcat hit in knownGrid
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
                // adaugam doar pe cei aflati in candidateMoves
                nearHits.AddRange(neighbors.Where(n => candidateMoves.Contains(n)));
            }

            // scoatem duplicatele
            nearHits = nearHits.Distinct().ToList();

            return nearHits;
        }

        // Returneaza vecini (sus, jos, stanga, dreapta)
        private List<(int, int)> GetNeighbors(int row, int col)
        {
            var result = new List<(int, int)>();
            if (row - 1 >= 0) result.Add((row - 1, col));
            if (row + 1 < GridSize) result.Add((row + 1, col));
            if (col - 1 >= 0) result.Add((row, col - 1));
            if (col + 1 < GridSize) result.Add((row, col + 1));
            return result;
        }

        // Returneaza un celula (vecina ultimei lovituri) necunoscuta (-1), daca exista
        private (int, int) GetCloseNeighbor(int[,] simulationGrid, int row, int col)
        {
            var neighbors = GetNeighbors(row, col);
            var candidates = new List<(int, int)>();
            foreach (var (r, c) in neighbors)
            {
                if (simulationGrid[r, c] == -1)
                {
                    candidates.Add((r, c));
                }
            }
            if (candidates.Count == 0) return (-1, -1);

            return candidates[rand.Next(candidates.Count)];
        }
    }
}