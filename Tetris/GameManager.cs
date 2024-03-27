using System;
using System.Collections.Generic;
using System.Threading;

namespace Tetris
{
    public class GameManager
    {
        public static GameManager Instance { get; private set; }

        // 게임의 흐름. 값 등을 관리하는 객체.
        int level;  // 레벨
        int score;  // 점수

        public int Level => level;
        public int Score => score;

        List<MonoBehaivor> loopList;

        public GameManager()
        {
            Instance = this;           

            loopList = new List<MonoBehaivor>();
            loopList.Add(new Background(10, 20));
            loopList.Add(new Block());
            loopList.Add(new UI());
        }

        public void StartGame()
        {
            Console.Clear();
            level = 1;
            score = 0;

            foreach (var loop in loopList)
                loop.Start();

            Thread.Sleep(Game.FIXED_MILLISEC);

            while (Block.Instance.IsActivate)
            {
                foreach (var loop in loopList)
                    loop.Update();

                foreach (var loop in loopList)
                    loop.Render();

                Thread.Sleep(Game.FIXED_MILLISEC);
            }

            Thread.Sleep(500);
            GameOver();
        }

        int cursor = 0;
        private void GameOver()
        {
            cursor = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆");
                Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
                Console.WriteLine("◆　　　　　　GAMEOVER　　　　　　◆");
                Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
                Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
                Console.WriteLine("◆　　　　　　　　　　　　　　　　◆");
                Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆");

                Console.SetCursorPosition(7 * 2, 3);
                Console.Write("{0}RETRY", cursor == 0 ? "▶" : "　");
                Console.SetCursorPosition(7 * 2, 4);
                Console.Write("{0}QUIT", cursor == 1 ? "▶" : "　");

                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow && cursor > 0)
                    cursor -= 1;
                else if (key == ConsoleKey.DownArrow && cursor < 1)
                    cursor += 1;
                else if(key == ConsoleKey.Enter)
                    break;
            }

            if(cursor == 0)
            {
                StartGame();
            }
            else
            {
                Console.SetCursorPosition(0, 8);
            }
        }
        
        // 블록이 설치되거나 라인이 삭제되면 점수가 오른다.
        public void AddScore(int line)
        {
            // 기본점수 : 블록 설치 시 +25, 라인 제거 시 개당 +100, 연속 제거시 줄당 +20%, 레벨 당 +10%.
            int amount = (int)MathF.Round((25 + line * 100) * (1 + 0.2f * line) * (1 + 0.1f * level));
            score += amount;
        }
    }
}
