using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TetrisGame;
using TetrisGA;
using System.Threading;

namespace Test {
    class Program {
        //static void Main(string[] args) {
        //    int[] a = new int[3] { 1, 2, 3};
        //    int[] b = new int[2] { 4, 5 };

        //    int[] c = new int[5];

        //    a.CopyTo(c, 0);
        //    b.CopyTo(c, 3);

        //    for (int i = 0; i < c.Length; i++) {
        //        Console.WriteLine(c[i]);
        //    }
        //}
        static void Main(string[] args) {
            TetrisAIManager tetrisAIManager = new TetrisAIManager(30);

            //tetrisAIManager.TetrisAIs[0].Gene = new int[9] { -5, 5, -3, 5, 5, 5, 0, 3, 2 };
            //tetrisAIManager.Genes[0] = new int[9] { -5, 5, -3, 5, 5, 5, 0, 3, 2 };

            //for (int i = 0; i < 1000; i++) {
            //    tetrisAIManager.PlaceMino();
            //}

            int c = 0;
            while (true) {
                //Thread.Sleep(10);
                tetrisAIManager.PlaceMino();

                if (++c >= 200) {
                    c = 0;
                    tetrisAIManager.NextGeneration();
                }

                //char key = Console.ReadKey().KeyChar;

                //if (key == ' ') {
                //    tetrisAIManager.PlaceMino();
                //} else if (key == 'r') {
                //    tetrisAIManager.NextGeneration();
                //}

                Console.SetCursorPosition(0, 0);
                Console.Write("          ");
                Console.SetCursorPosition(0, 0);
                Console.Write($"{tetrisAIManager.Generation} {c} {tetrisAIManager.Tetrises[0].Score}");

                for (int k = 0; k < 0; k++) {
                    for (int i = 0; i < 20; i++) {
                        for (int j = 0; j < 10; j++) {
                            Console.SetCursorPosition(j + k * 14, i);
                            if (tetrisAIManager.Tetrises[k].PlayField[i, j] != MinoType.None) {
                                Console.Write(tetrisAIManager.Tetrises[k].PlayField[i, j]);
                            } else {
                                Console.Write(" ");
                            }
                        }
                    }
                    Console.SetCursorPosition(k*14, 20);
                    Console.WriteLine(tetrisAIManager.Tetrises[k].Score);
                    for (int i = 0; i < 9; i++) {
                        Console.SetCursorPosition(k * 14, 21 + i);
                        Console.WriteLine("    ");
                        Console.SetCursorPosition(k * 14, 21 + i);
                        Console.WriteLine(tetrisAIManager.Genes[k][i]);
                    }
                }

                //Console.SetCursorPosition(84, 0);
                //Console.WriteLine(tetrisAIManager.Generation);
                //Console.SetCursorPosition(84, 1);
                //Console.WriteLine(c);
            }
        }
    }
}
