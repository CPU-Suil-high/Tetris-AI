using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisGame;

namespace TetrisGA {
    [Serializable]
    public class TetrisAIManager {
        public static readonly int MaxWeight = 1000;
        public static readonly int MinWeight = -1000;

        private Random random;
        public Random Random {
            get {
                return random;
            }
            set {
                random = value;
            }
        }

        private int seed;
        public int Seed {
            get {
                return seed;
            }
            set {
                seed = value;
            }
        }

        private int count;
        public int Count {
            get {
                return count;
            }
            set {
                count = value;
            }
        }

        private int generation = 0;
        public int Generation {
            get {
                return generation;
            }
            set {
                generation = value;
            }
        }

        private TetrisAI[] tetrisAIs;
        public TetrisAI[] TetrisAIs {
            get {
                return tetrisAIs;
            }
            set {
                tetrisAIs = value;
            }
        }

        private Tetris[] tetrises;
        public Tetris[] Tetrises {
            get { 
                return tetrises;
            }
            set {
                tetrises = value;
            }
        }

        private int[][] genes;
        public int[][] Genes {
            get {
                return genes;
            }
            set {
                genes = value;
            }
        }

        public TetrisAIManager(int count) {
            Random = new Random();
            Seed = Random.Next(0, int.MaxValue);

            if (count < 10) {
                Count = 10;
            } else {
                Count = count;
            }

            Tetrises = new Tetris[Count];
            TetrisAIs = new TetrisAI[Count];
            Genes = new int[Count][];

            for (int i = 0; i < Count; i++) {
                Tetrises[i] = new Tetris(Seed);
                Genes[i] = GetRandomGene();
                TetrisAIs[i] = new TetrisAI(Tetrises[i], Genes[i]);
            }
        }

        public void PlaceMino() {
            for (int i = 0; i < Count; i++) {
                TetrisAIs[i].PlaceMino();
            }
        }

        public int[] GetRandomGene() {
            int[] gene = new int[9];

            for (int i = 0; i < 9; i++) {
                gene[i] = Random.Next(MinWeight, MaxWeight+1);
            }

            return gene;
        }

        public int[][] GetBestGenes(int n) {
            int[][] genes = new int[n][];

            TetrisAI[] tetrisAIs = new TetrisAI[Count];

            for (int i = 0; i < Count; i++) {
                tetrisAIs[i] = (TetrisAI)TetrisAIs[i].Clone();
            }

            Array.Sort(tetrisAIs, (a, b) => {
                int aScore = a.Tetris.Score * 2 + a.Tetris.PlaceCount;
                int bScore = b.Tetris.Score * 2 + b.Tetris.PlaceCount;

                if (aScore == bScore) {
                    return 0;
                }

                return (aScore < bScore) ? 1 : -1;
            });

            for (int i = 0; i < n; i++) {
                genes[i] = tetrisAIs[i].Gene;
            }

            return genes;
        }
        public int[][] GetRandomGenes(int n) {
            int[][] genes = new int[n][];

            for (int i = 0; i < n; i++) {
                int index = Random.Next(0, Count);
                genes[i] = (int[])Genes[index].Clone();
            }

            return genes;
        }

        public int[][] CrossOver(int[][] genes, int n) {
            int[][] children = new int[n][];

            for (int i = 0; i < n; i++) {
                int index1 = Random.Next(0, genes.Length);
                int index2 = Random.Next(0, genes.Length);
                int[] gene1 = genes[index1];
                int[] gene2 = genes[index2];

                int[] child = new int[9];

                for (int j = 0; j < 9; j++) {
                    if (Random.Next(0, 100) < 50) {
                        child[j] = gene1[j];
                    } else {
                        child[j] = gene2[j];
                    }
                }

                children[i] = child;
            }

            return children;
        }

        public int[][] Mutation(int[][] genes) {
            genes = (int[][])genes.Clone();
            for (int i = 0; i < genes.GetLength(0); i++) {
                for (int j = 0; j < 9; j++) {
                    int num = Random.Next(0, 100);

                    if (num < 1) {
                        genes[i][j] = Random.Next(MinWeight, MaxWeight+1);
                    } else if (num < 6) {
                        genes[i][j] += Random.Next(1, 6);
                        if (genes[i][j] > MaxWeight) {
                            genes[i][j] = MaxWeight;
                        }
                    } else if (num < 11) {
                        genes[i][j] -= Random.Next(1, 6);
                        if (genes[i][j] < MinWeight) {
                            genes[i][j] = MinWeight;
                        }
                    }
                }
            }

            return genes;
        }

        public void NextGeneration() {
            Random = new Random();

            Generation++;
            Seed = Random.Next(0, int.MaxValue);

            int[][] bestGenes = GetBestGenes(4);
            int[][] randomGenes = GetRandomGenes(1);

            int[][] temp = new int[5][];

            bestGenes.CopyTo(temp, 0);
            randomGenes.CopyTo(temp, 4);

            int[][] children = CrossOver(temp, Count - 4);
            children = Mutation(children);

            bestGenes.CopyTo(Genes, 0);
            children.CopyTo(Genes, 4);

            for (int i = 0; i < Count; i++) {
                TetrisAIs[i].Gene = Genes[i];
                Tetrises[i].ReSet(Seed);
            }
        }

        public bool CheckAllIsGameOver() {
            for (int i = 0; i < Count; i++) {
                if (!Tetrises[i].IsGameOver) {
                    return false;
                }
            }

            return true;
        }
    }
}
