using System;
using System.Collections.Generic;
using System.Threading;

namespace Tetris
{
    static class Sign
    {
        public static readonly string WALL = "■";
        public static readonly string BLOCK = "▣";
        public static readonly string BLANK = "　";
    }

    public static class Game
    {
        public static int FPS;                  // 프레임.
        public static float FIXED_DELTA_TIME;   // 고정 델타 타임.
        public static int FIXED_MILLISEC;       // 고정 밀리 타임.

        static Game()
        {
            FPS = 12;
            FIXED_DELTA_TIME = MathF.Round((1000f / FPS) / 1000f, 1);
            FIXED_MILLISEC = (1000 / FPS);

            Console.CursorVisible = false;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆");
            Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
            Console.WriteLine("◆　　　　　　TETRIS　　　　　　◆");
            Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
            Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
            Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
            Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆");


            GameManager manager = new GameManager();
            manager.StartGame();
        }
    }
}
