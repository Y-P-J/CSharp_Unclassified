using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public static class Input
    {
        static ConsoleKey inputKey = 0;

        public static bool GetKey(ConsoleKey keyCode)
        {
            // 사용자가 키입력을 했는가?
            if(Console.KeyAvailable)
                inputKey = Console.ReadKey(true).Key;

            if(inputKey == keyCode)
            {
                // 입력키를 초기화하고 입력 버퍼를 비운다.
                inputKey = 0;
                while (Console.KeyAvailable)
                    Console.ReadKey();

                return true;
            }

            return false;
        }
    }
}
