using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class Background : MonoBehaivor
    {
        class Line
        {
            bool[] blocks;     // 해당 라인의 데이터.

            // Count : 조건식을 만족하는 개수
            public bool IsFull => blocks.Count(b => b) >= WIDTH;

            public Line()
            {
                blocks = new bool[WIDTH];
            }
            public void Set(int index)
            {
                blocks[index] = true;
            }            
            public bool IsSet(int index)
            {
                return blocks[index];
            }
        }

        // 싱클턴 패턴
        // => 오직 1개만 존재하고 접근이 잦을 때 사용한다. (매니저)
        public static Background Instance { get; private set; }

        public static int WIDTH { get; private set; }
        public static int HEIGHT { get; private set; }

        // 블록이 쌓이는 공간.
        private List<Line> lines;

        public Background(int width, int height)
        {
            Instance = this;

            WIDTH = width;
            HEIGHT = height;
        }

        public override void Start()
        {
            // 라인 배열 생성 후 각 i번째 라인 객체 할당.
            lines = new List<Line>();
            for (int i = 0; i < HEIGHT; i++)
                lines.Add(new Line());
        }
        public override void Render()
        {
            Console.SetCursorPosition(0, 0);
            for (int row = 0; row < HEIGHT + 1; row++)
            {
                for (int column = 0; column < WIDTH + 2; column++)
                {
                    if (column == 0 || column == WIDTH + 1 || row == HEIGHT)
                        Console.Write(Sign.WALL);
                    else
                        Console.Write(Sign.BLANK);
                }
                Console.WriteLine();
            }

            // row, column위치에 설치된 블록이 있을 경우.
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (lines[y].IsSet(x))
                    {
                        Console.SetCursorPosition((x + 1) * 2, y);
                        Console.Write(Sign.BLOCK);
                    }
                }
            }

          
        }

        // 해당 포지션들이 백그라운드 내부에서 충돌을 일으키는가?
        public bool IsCollision(Vector2[] positions)
        {
            foreach(Vector2 pos in positions)
            {
                if (pos.x < 0 || pos.x >= WIDTH)        // 수평 한계
                    return true;
                if (pos.y < 0 || pos.y >= HEIGHT)       // 수직 한계
                    return true;
                if (lines[pos.y].IsSet(pos.x))          // 설치된 블록
                    return true;
            }

            return false;
        }
        public void SetBlock(Vector2[] positions)
        {
            // 좌표 배열을 이용해 각 라인에 블록 세팅.
            foreach(Vector2 p in positions)
                lines[p.y].Set(p.x);

            // 각 라인별로 블록이 다 차있는지 검사 후 삭제.
            int removeLine = 0;
            for(int i = 0; i<HEIGHT; i++)
            {                
                if (lines[i].IsFull)                    // i번째 라인이 다 차있는 경우.
                {   
                    lines.RemoveAt(i);                  // i번째 라인 삭제.
                    lines.Insert(0, new Line());        // 0번째 라인 삽입.
                    removeLine++;
                }
            }

            GameManager.Instance.AddScore(removeLine);
        }
    }
}
