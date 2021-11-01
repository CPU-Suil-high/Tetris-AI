using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisGame;

namespace TetrisGA {
    [Serializable]
    public class TetrisAI : ICloneable {

        private Tetris tetris;
        public Tetris Tetris {
            get {
                return tetris;
            }
            set {
                tetris = value;
            }
        }

        private int[] gene;
        public int[] Gene {
            get {
                return gene;
            }
            set {
                gene = value;
            }
        }

        public TetrisAI(Tetris tetris, int[] gene) {
            Tetris = tetris;
            Gene = gene;
        }

        public void PlaceMino() {
            if (Tetris.IsGameOver) {
                return;
            }

            Tetris.SetTetris(GetMaxWeightTetris());
        }

        private Tetris GetMaxWeightTetris() {
            Tetris maxTetris = GetMaxWeightTetrisFromRotation(0);
            int maxWeight = GetWeight(maxTetris);

            for (int i = 1; i < 4; i++) {
                Tetris temp = GetMaxWeightTetrisFromRotation(i);
                int weight = GetWeight(temp);

                if (weight > maxWeight) {
                    maxWeight = weight;
                    maxTetris = temp;
                }
            }

            return maxTetris;
        }

        private Tetris GetMaxWeightTetrisFromRotation(int rotation) {
            Tetris tetris = (Tetris)Tetris.Clone();

            for (int i = 0; i < rotation; i++) {
                tetris.RotateClockwise();
            }

            while (!tetris.CheckMinoCollision()) {
                tetris.MoveLeft();
            }
            tetris.MoveRight();

            Tetris maxTetris = (Tetris)tetris.Clone();
            maxTetris.DropHard();
            int maxWeight = GetWeight(maxTetris);

            tetris.MoveRight();

            while (!tetris.CheckMinoCollision()) {
                Tetris temp = (Tetris)tetris.Clone();
                temp.DropHard();

                int weight = GetWeight(temp);

                if (weight > maxWeight) {
                    maxWeight = weight;
                    maxTetris = temp;
                }

                tetris.MoveRight();
            }

            return maxTetris;
        }

        private int GetWeight(Tetris tetris) {
            int weight = 0;

            weight += tetris.GetEmptyBlockWithCeilingCount() * Gene[0];
            weight += tetris.GetHighestEmptyBlockY() * Gene[1];
            weight += tetris.GetHighestEmptyBlockCeilingCount() * Gene[2];
            weight += tetris.GetHighestBlockYFromLines() * Gene[3];
            weight += tetris.GetHighestBlocksYAverage() * Gene[4];
            weight += tetris.GetHighestBlocksYStdDev() * Gene[5];
            weight += tetris.GetDifferenceBetweenMaxAndMin() * Gene[6];
            weight += tetris.GetBlockOnWallCount() * Gene[7];
            weight += tetris.Score * Gene[8];

            return weight;
        }

        public object Clone() {
            return new TetrisAI((Tetris)Tetris.Clone(), (int[])Gene.Clone());
        }
    }
}
