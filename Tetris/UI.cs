using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class UI : MonoBehaivor
    {
        int WIDTH = 10;
        int HEIGHT = 10;

        Vector2 startPos;

        public override void Start()
        {
            startPos = new Vector2((WIDTH + 2), 1);
        }

        // UI를 그린다.
        public override void Render()
        {
            Clear();
            SetCursor(2, 0);
            Console.Write("NEXT");

            // 다음 블록을 그린다.
            Shape nextShape = BlockHandler.GetNextShape();
            Console.ForegroundColor = nextShape.color;
            foreach(Vector2 pos in nextShape.GetActivePosition(0))
            {
                SetCursor(pos.x, pos.y);
                Console.Write(Sign.BLOCK);
            }
            Console.ForegroundColor = ConsoleColor.White;

            // 레벨, 점수를 그린다.
            SetCursor(1, 14);
            Console.Write($"LEVEL {GameManager.Instance.Level}");
            SetCursor(1, 16);
            Console.Write($"SCORE {GameManager.Instance.Score}");
        }

        private void SetCursor(int x, int y)
        {
            // UI좌표계 상에서의 x,y위치로 커서를 이동한다.
            Console.SetCursorPosition((startPos.x + x) * 2, startPos.y + y);
        }
        private void Clear()
        {
            // UI좌표계상의 위치 전체에 BLANK블록을 그린다.
            for(int line = 0; line < HEIGHT; line++)
            {
                SetCursor(0, line);
                for (int i = 0; i < WIDTH; i++)
                    Console.Write(Sign.BLANK);
            }
        }

    }
}
