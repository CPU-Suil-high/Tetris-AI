using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    public enum MinoType {
        None,
        I,
        O,
        T,
        L,
        J,
        S,
        Z
    }
    public class Tetris : ICloneable
    {

        private int seed;
        public int Seed {
            get {
                return seed;
            }
            set {
                seed = value;
            }
        }
        public Random Random {
            get {
                PlaceCount++;
                return new Random(PlaceCount + Seed);
            }
        }
        private int placeCount = 0;
        public int PlaceCount {
            get {
                return placeCount;
            }
            set {
                placeCount = value;
            }
        }

        private MinoType[,] playField = new MinoType[20, 10];
        public MinoType[,] PlayField {
            get {
                return playField;
            }
            set {
                playField = value;
            }
        }

        private Mino curMino;
        public Mino CurMino {
            get {
                return curMino;
            }

            set {
                curMino = value;
            }
        }

        private MinoType curMinoType;
        public MinoType CurMinoType {
            get {
                return curMinoType;
            }
            set {
                curMinoType = value;
            }
        }

        private List<MinoType> bag;
        public List<MinoType> Bag {
            get {
                return bag;
            }
            set {
                bag = value;
            }
        }

        private List<MinoType> nextMinos;
        public List<MinoType> NextMinos {
            get {
                return nextMinos;
            }
            set {
                nextMinos = value;
            }
        }

        private int score;
        public int Score {
            get {
                return score;
            }
            set {
                score = value;
            }
        }

        private int combo;
        public int Combo {
            get {
                return combo;
            }
            set {
                combo = value;
            }
        }

        private bool isGameOver = false;
        public bool IsGameOver {
            get {
                return isGameOver;
            }
            private set {
                isGameOver = value;
            }
        }

        public Tetris() {
            Seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            Bag = new List<MinoType>();
            NextMinos = new List<MinoType>();

            for (int i = 0; i < 5; i++) {
                NextMinos.Add(GetRandomMinoType());
            }

            CurMino = GetNextMino();
        }

        public Tetris(int seed) {
            Seed = seed;
            Bag = new List<MinoType>();
            NextMinos = new List<MinoType>();

            for (int i = 0; i < 5; i++) {
                NextMinos.Add(GetRandomMinoType());
            }

            CurMino = GetNextMino();
        }

        MinoType GetRandomMinoType() {
            if (bag.Count == 0) {
                bag = new List<MinoType> { MinoType.I, MinoType.O, MinoType.T, MinoType.L, MinoType.J, MinoType.S, MinoType.Z };
            }

            int index = Random.Next(0, Bag.Count);
            MinoType minoType = Bag[index];
            Bag.RemoveAt(index);

            return minoType;
        }

        Mino GetNextMino() {
            MinoType minoType = NextMinos[0];
            
            NextMinos.RemoveAt(0);
            NextMinos.Add(GetRandomMinoType());

            Mino mino;

            switch (minoType) {
                case MinoType.I:
                    mino = new MinoI();
                    break;
                case MinoType.O:
                    mino = new MinoO();
                    break;
                case MinoType.T:
                    mino = new MinoT();
                    break;
                case MinoType.L:
                    mino = new MinoL();
                    break;
                case MinoType.J:
                    mino = new MinoJ();
                    break;
                case MinoType.S:
                    mino = new MinoS();
                    break;
                case MinoType.Z:
                    mino = new MinoZ();
                    break;
                default:
                    return null;
            }

            if (minoType == MinoType.I) {
                mino.Position = new Point(2, -3);
            } else {
                mino.Position = new Point(3, -3);
            }
            return mino;
        }

        public bool CheckMinoCollision() {
            for (int i = 0; i < CurMino.Size; i++) {
                int y = CurMino.Position.Y + i;

                for (int j = 0; j < CurMino.Size; j++) {
                    int x = CurMino.Position.X + j;

                    if (CurMino.Blocks[i, j] == MinoType.None) {
                        continue;
                    }
                    // PlayField 범위를 넘어갔을 때 처리
                    if (x < 0 || x >= PlayField.GetLength(1) || y >= PlayField.GetLength(0)) {
                        return true;
                    } else if (y < 0) {
                        continue;
                    }

                    // 겹치는지 확인
                    if (PlayField[y, x] != MinoType.None) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Update() {
            MoveDown();
            if (CheckMinoCollision()) {
                MoveUp();
                Place();
            }
        }

        public void MoveLeft() {
            Point pos = CurMino.Position;
            pos.X--;
            CurMino.Position = pos;
        }

        public void MoveRight() {
            Point pos = CurMino.Position;
            pos.X++;
            CurMino.Position = pos;
        }

        public void MoveDown() {
            Point pos = CurMino.Position;
            pos.Y++;
            CurMino.Position = pos;
        }

        public void MoveUp() {
            Point pos = CurMino.Position;
            pos.Y--;
            CurMino.Position = pos;
        }

        public bool CheckLine(int y) {
            for (int x = 0; x < 10; x++) {
                if (PlayField[y, x] == MinoType.None) {
                    return false;
                }
            }
            return true;
        }

        public void RemoveLine(int y) {
            for (; y > 0; y--) {
                for (int x = 0; x < 10; x++) {
                    PlayField[y, x] = PlayField[y - 1, x];
                }
            }
            for (int x = 0; x < 10; x++) {
                PlayField[0, x] = MinoType.None;
            }
        }

        public void RemoveLines() {
            int count = 0;
            for (int y = 0; y < 20; y++) {
                if (CheckLine(y)) {
                    count += 1;
                    RemoveLine(y);
                }
            }

            if (count > 1) {
                int score = (int)Math.Pow(2, count - 2);

                if (Combo < 2) {

                } else if (Combo < 4) {
                    score += 1;
                } else if (Combo < 6) {
                    score += 2;
                } else if (Combo < 8) {
                    score += 3;
                } else if (Combo < 11) {
                    score += 4;
                } else {
                    score += 5;
                }

                Score += score;
                Combo += 1;
            } else {
                Combo = 0;
            }
        }

        public void Place() {
            if (CheckMinoCollision()) {
                return;
            }

            for (int i = 0; i < CurMino.Size; i++) {
                int y = CurMino.Position.Y + i;

                for (int j = 0; j < CurMino.Size; j++) {
                    int x = CurMino.Position.X + j;

                    if (CurMino.Blocks[i, j] == MinoType.None || x < 0 || x >= PlayField.GetLength(1) || y >= PlayField.GetLength(0)) {
                        continue;
                    } else if (y < 0) {
                        IsGameOver = true;
                        continue;
                    }

                    PlayField[y, x] = CurMino.Blocks[i, j];
                }
            }

            CurMino = GetNextMino();
            RemoveLines();
        }

        public void DropSoft() {

        }

        public void DropHard() {
            while (!CheckMinoCollision()) {
                MoveDown();
            }
            MoveUp();
            Place();
        }

        public void RotateClockwise() {
            int preRotation;
            int curRotation;

            Point point = CurMino.Position;

            preRotation = CurMino.Rotation;
            CurMino.RotateClockwise();
            curRotation = CurMino.Rotation;
            
            for (int i = 0; i < 4; i++) {
                CurMino.Position = point + (Size)CurMino.Offset[preRotation, i] - (Size)CurMino.Offset[curRotation, i];

                if (!CheckMinoCollision()) {
                    return;
                }
            }
            CurMino.RotateCounterclockwise();
            CurMino.Position = point;
        }

        public void RotateCounterclockwise() {
            int preRotation;
            int curRotation;

            Point point = CurMino.Position;

            preRotation = CurMino.Rotation;
            CurMino.RotateCounterclockwise();
            curRotation = CurMino.Rotation;

            for (int i = 0; i < 4; i++) {
                CurMino.Position = point + (Size)CurMino.Offset[preRotation, i] - (Size)CurMino.Offset[curRotation, i];

                if (!CheckMinoCollision()) {
                    return;
                }
            }
            CurMino.RotateClockwise();
            CurMino.Position = point;
        }

        public void Hold() {

        }
        
        public bool CheckHavingCeiling(int x, int y) {
            for (y = y - 1; y >= 0; y--) {
                if (PlayField[y, x] != MinoType.None) {
                    return true;
                }
            }

            return false;
        }

        // 천장에 막힌 빈블럭 개수
        public int GetEmptyBlockWithCeilingCount() {
            int count = 0;

            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 10; j++) {
                    if (PlayField[i, j] == MinoType.None && CheckHavingCeiling(j, i)) {
                        count++;
                    }
                }
            }

            return count;
        }

        // 가장 높은 빈블럭 y
        public int GetHighestEmptyBlockY() {
            for (int y = 0; y < 20; y++) {
                for (int x = 0; x < 10; x++) {
                    if (PlayField[y, x] == MinoType.None && CheckHavingCeiling(x, y)) {
                        return y;
                    }
                }
            }
            return 20;
        }

        // 가장 위에 있는 천장있는 빈블럭의 천장 수
        public int GetHighestEmptyBlockCeilingCount() {
            int count = 0;

            for (int y = 0; y < 20; y++) {
                if (count > 0) {
                    break;
                }
                for (int x = 0; x < 10; x++) {
                    if (PlayField[y, x] == MinoType.None && CheckHavingCeiling(x, y)) {
                        for (int i = y-1; i > 0; i--) {
                            if (PlayField[i, x] == MinoType.None) {
                                break;
                            }
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public int GetHighestBlockYFromLine(int x) {
            for (int y = 0; y < 20; y++) {
                if (PlayField[y, x] != MinoType.None) {
                    return y;
                }
            }
            return 20;
        }

        // 가장 높은 블럭 높이
        public int GetHighestBlockYFromLines() {
            int max = 20;

            for (int x = 0; x < 10; x++) {
                int y = GetHighestBlockYFromLine(x);

                if (y < max) {
                    max = y;
                }
            }

            return max;
        }

        // x좌표별 가장 높은 블럭 높이 평균
        public int GetHighestBlocksYAverage() {
            int sum = 0;

            for (int x = 0; x < 10; x++) {
                sum += GetHighestBlockYFromLine(x);
            }

            return (int)Math.Round((double)sum / 10);
        }

        // x좌표별 가장 높은 블럭 높이 표준편차
        public int GetHighestBlocksYStdDev() {
            int m = GetHighestBlocksYAverage();
            double v = 0;

            for (int x = 0; x < 10; x++) {
                int dev = GetHighestBlockYFromLine(x) - m;
                v += dev * dev;
            }

            v /= 10;

            return (int)Math.Round(Math.Sqrt(v));
        }

        // x좌표별 높이 최대 최소의 차
        public int GetDifferenceBetweenMaxAndMin() {
            int min = GetHighestBlockYFromLines();
            int max = 0;

            for (int x = 0; x < 10; x++) {
                int y = GetHighestBlockYFromLine(x);
                if (y > max) {
                    max = y;
                }
            }

            return max - min;
        }

        // 벽에 있는 블럭 개수
        public int GetBlockOnWallCount() {
            int count = 0;

            for (int y = 0; y < 20; y++) {
                if (PlayField[y, 0] != MinoType.None) {
                    count++;
                }

                if (PlayField[y, 9] != MinoType.None) {
                    count++;
                }
            }

            return count;
        }

        public object Clone() {
            Tetris tetris = new Tetris();

            tetris.Seed = Seed;
            tetris.PlaceCount = PlaceCount;
            tetris.PlayField = (MinoType[,])PlayField.Clone();
            tetris.CurMino = (Mino)CurMino.Clone();
            tetris.CurMinoType = CurMinoType;
            tetris.Bag = Bag.ToList();
            tetris.NextMinos = NextMinos.ToList();
            tetris.Combo = Combo;
            tetris.Score = Score;
            tetris.IsGameOver = IsGameOver;

            return tetris;
        }

        public void SetTetris(Tetris tetris) {
            Seed = tetris.Seed;
            PlaceCount = tetris.PlaceCount;
            PlayField = (MinoType[,])tetris.PlayField.Clone();
            CurMino = (Mino)tetris.CurMino.Clone();
            CurMinoType = tetris.CurMinoType;
            Bag = tetris.Bag.ToList();
            NextMinos = tetris.NextMinos.ToList();
            Combo = tetris.Combo;
            Score = tetris.Score;
            IsGameOver = tetris.IsGameOver;
        }

        public void ReSet() {
            Tetris tetris = new Tetris();

            SetTetris(tetris);
        }

        public void ReSet(int seed) {
            Tetris tetris = new Tetris(seed);

            SetTetris(tetris);
        }
    }
}
