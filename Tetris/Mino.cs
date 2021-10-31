using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame {
    public class OffsetDatas {
        public static Point[,] OffsetJLSTZ = new Point[4, 5] {
        {new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
        {new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, -2), new Point(1, -2) },
        {new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
        {new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(0, -2), new Point(-1, -2) }
        };

        public static Point[,] OffsetI = new Point[4, 5] {
        {new Point(0, 0), new Point(-1, 0), new Point(2, 0), new Point(-1, 0), new Point(2, 0) },
        {new Point(-1, 0), new Point(0, 0), new Point(0, 0), new Point(0, -1), new Point(0, 2) },
        {new Point(-1, -1), new Point(1, -1), new Point(-2, -1), new Point(1, 0), new Point(-2, 0) },
        {new Point(0, -1), new Point(0, -1), new Point(0, -1), new Point(0, 1), new Point(0, -2) }
        };

        public static Point[,] OffsetO = new Point[4, 5] {
        {new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
        {new Point(0, 1), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
        {new Point(-1, 1), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
        {new Point(-1, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) }
        };
    }
    public class Mino : ICloneable {
        private MinoType[,] blocks;
        public MinoType[,] Blocks {
            get {
                return blocks;
            }
            set {
                blocks = value;
                Size = blocks.GetLength(0);
            }
        }

        private int size;
        public int Size {
            get {
                return size;
            }

            set {
                size = value;
            }
        }

        private Point position;
        public Point Position {
            get {
                return position;
            }

            set {
                position = value;
            }
        }

        private int rotation;
        public int Rotation {
            get {
                return rotation;
            }

            set {
                rotation = value % 4;

                if (rotation < 0) {
                    rotation = rotation + 4;
                }
            }
        }

        private Point[,] offset;
        public Point[,] Offset {
            get {
                return offset;
            }
            set {
                offset = value;
            }
        }

        public Mino() {
            Blocks = new MinoType[3, 3];
        }

        public void RotateClockwise() {
            MinoType[,] temp = new MinoType[Size, Size];

            for (int i = 0; i < Size; i++) {
                for (int j = 0; j < Size; j++) {
                    temp[j, Size - i - 1] = Blocks[i, j];
                }
            }

            Blocks = temp;
            Rotation = Rotation + 1;
        }

        public void RotateCounterclockwise() {
            MinoType[,] temp = new MinoType[Size, Size];

            for (int i = 0; i < Size; i++) {
                for (int j = 0; j < Size; j++) {
                    temp[i, j] = Blocks[j, Size - i - 1];
                }
            }

            Blocks = temp;
            Rotation = Rotation - 1;
        }

        public object Clone() {
            Mino mino = new Mino();

            mino.Size = Size;
            mino.Blocks = (MinoType[,])Blocks.Clone();
            mino.Rotation = Rotation;
            mino.Offset = Offset;
            mino.Position = Position;

            return mino;
        }
    }

    public class MinoI : Mino {
        public MinoI() : base() {
            Blocks = new MinoType[5, 5] {
                {MinoType.None, MinoType.None, MinoType.None, MinoType.None, MinoType.None},
                {MinoType.None, MinoType.None, MinoType.None, MinoType.None, MinoType.None},
                {MinoType.None, MinoType.I, MinoType.I, MinoType.I, MinoType.I},
                {MinoType.None, MinoType.None, MinoType.None, MinoType.None, MinoType.None},
                {MinoType.None, MinoType.None, MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetI;
        }
    }

    public class MinoJ : Mino {
        public MinoJ() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.J, MinoType.None, MinoType.None},
                {MinoType.J, MinoType.J, MinoType.J},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetJLSTZ;
        }
    }

    public class MinoL : Mino {
        public MinoL() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.None, MinoType.None, MinoType.L},
                {MinoType.L, MinoType.L, MinoType.L},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetJLSTZ;
        }
    }

    public class MinoO : Mino {
        public MinoO() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.None, MinoType.O, MinoType.O},
                {MinoType.None, MinoType.O, MinoType.O},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetO;
        }
    }

    public class MinoS : Mino {
        public MinoS() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.None, MinoType.S, MinoType.S},
                {MinoType.S, MinoType.S, MinoType.None},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetJLSTZ;
        }
    }

    public class MinoT : Mino {
        public MinoT() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.None, MinoType.T, MinoType.None},
                {MinoType.T, MinoType.T, MinoType.T},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetJLSTZ;
        }
    }

    public class MinoZ : Mino {
        public MinoZ() : base() {
            Blocks = new MinoType[3, 3] {
                {MinoType.Z, MinoType.Z, MinoType.None},
                {MinoType.None, MinoType.Z, MinoType.Z},
                {MinoType.None, MinoType.None, MinoType.None}
            };

            Offset = OffsetDatas.OffsetJLSTZ;
        }
    }
}
