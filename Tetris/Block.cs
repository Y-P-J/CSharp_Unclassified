using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Tetris.Block;

namespace Tetris
{
    public struct Vector2
    {
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public int x;
        public int y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator+(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator *(Vector2 v1, int scalar)
        {
            return new Vector2(v1.x * scalar, v1.y * scalar);
        }
        public override string ToString()
        {
            return $"({x},{y})";
        }
    }
    enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
    enum SHAPE
        {
            O,
            I,
            S,
            Z,
            L,
            J,
            T,

            COUNT,
        }

    class Shape
    {
        public SHAPE shape;
        public ConsoleColor color;
        public short[,] datas;
        public int rotation;

        public Vector2[] GetActivePosition(bool isRotate = false)
        {
            int rotation = this.rotation;
            if(isRotate)
            {
                rotation += 1;
                if (rotation >= datas.GetLength(0))
                    rotation = 0;
            }

            return GetActivePosition(rotation);
        }
        public Vector2[] GetActivePosition(int rotation)
        {
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < datas.GetLength(1); i++)
            {
                if (datas[rotation, i] == 1)
                    list.Add(new Vector2(i % Block.WIDTH, i / Block.WIDTH));
            }
            return list.ToArray();
        }

        public void Rotate()
        {
            rotation += 1;
            if (rotation >= datas.GetLength(0))
                rotation = 0;
            /*
            // L - (W * (y + 1)) + x
            short[] newDatas = new short[datas.Length];
            for(int i = 0; i<newDatas.Length; i++)
            {
                int row = i / Block.WIDTH;
                int column = i % Block.WIDTH;
                int index = datas.Length - (Block.WIDTH * (column + 1)) + row;
                newDatas[index] = datas[i];
            }

            datas = newDatas;
            */
        }                
    }
    class Block : MonoBehaivor  
    {
        public static Block Instance { get; private set; }

        public static int WIDTH = 4;    // 너비.
        public static int HEIGHT = 4;   // 높이.

        public bool IsActivate { get; private set; }

        Vector2 position;       // 위치 값.
        Shape shape;            // 모양.
        int rotation;           // 회전 값.
        int speed;              // 속도 값.                

        float waitTime;
        float time;


        public override void Start()
        {
            waitTime = 1.0f;
            time = 0.0f;

            Instance = this;
            IsActivate = true;

            ChangeRandomShape();
        }

        public override void Update()
        {
            if (shape == null)
                return;

            UpdateMovement();         // 이동.
            UpdateDirectMovement();   // 즉시 이동.
        }
        public override void Render()
        {
            if (shape == null)
                return;

            ConsoleColor beforeColor = Console.ForegroundColor;     // 이전 색상 컬러 저장.
            Console.ForegroundColor = shape.color;                  // 모양에 맞는 색상 변경.

            Vector2[] actives = GetActivePosition();                // 활성화 상태의 위치 값.
            foreach(Vector2 pos in actives)                         
            {
                Console.SetCursorPosition((pos.x + 1) * 2, pos.y);  // 위치 값에 맞게 Cursor 이동.
                Console.Write(Sign.BLOCK);                          // BLOCK 그리기.
            }

            Console.ForegroundColor = beforeColor;                  // 이전 색상 값으로 되돌리기.
        }

        private void UpdateMovement()
        {
            // 키보드 왼쪽, 오른쪽, 아래 키를 누르면 블록이 좌,우, 아래로 1칸씩 이동한다.
            if (Input.GetKey(ConsoleKey.DownArrow))
            {
                if (IsMove(DIRECTION.DOWN))
                {
                    time = 0f;
                    position.y += 1;
                }
                else
                    SetBlockToBackground();
            }
            else if (Input.GetKey(ConsoleKey.LeftArrow))
            {
                if (IsMove(DIRECTION.LEFT))
                    position.x -= 1;
            }
            else if (Input.GetKey(ConsoleKey.RightArrow))
            {
                if (IsMove(DIRECTION.RIGHT))
                    position.x += 1;
            }
            else if (Input.GetKey(ConsoleKey.UpArrow))
            {
                Vector2[] rotateActivePosition = GetActivePosition(true);
                if(!Background.Instance.IsCollision(rotateActivePosition))
                    shape.Rotate();
            }

            // time에 시간값을 더하다가 원하는 시간만큼 대기했다면..
            // 시간 값을 뺀 후 y축 위치를 1 내리자.
            if ((time += Game.FIXED_DELTA_TIME) >= waitTime)
            {
                time -= waitTime;
                if (IsMove(DIRECTION.DOWN))
                    position.y += 1;
                else
                    SetBlockToBackground();
            }
        }
        private void UpdateDirectMovement()
        {
            if (Input.GetKey(ConsoleKey.Spacebar))
            {
                time = 0f;

                // 더 이상 아래로 내려갈 수 없을 때까지 가상 이동.
                while (IsMove(DIRECTION.DOWN))
                    position.y += 1;

                SetBlockToBackground();
            }
        }
        
        private Vector2[] GetActivePosition(bool isRotate = false)
        {
            // 위치 값 보정.
            Vector2[] actives = shape.GetActivePosition(isRotate);
            for (int i = 0; i < actives.Length; i++)
                actives[i] = position + actives[i];
            return actives;
        }                

        private bool IsMove(DIRECTION dir, int distance = 1)
        {            
            Vector2[] activePositions = GetActivePosition();            // 활성화 좌표 배열.
            BlockHandler.MoveVector(activePositions, dir, distance);    // 좌표 배열 이동. 
            return !Background.Instance.IsCollision(activePositions);   // 충돌 체크.
        }
        private void SetBlockToBackground()
        {
            Vector2[] activeDots = GetActivePosition();     // 활성화 좌표 배열.
            Background.Instance.SetBlock(activeDots);       // Background에 블록 설치.
            ChangeRandomShape();                            // 블록 모양 랜덤 변경.
        }
        private void ChangeRandomShape()
        {
            // 랜덤한 모양을 BlockHandler에게서 받아와 세팅한다.
            position = new Vector2(Background.WIDTH / 2 - 2, 0);
            shape = BlockHandler.GetRandomShape();

            // 모양 변경 후 충돌체크를 시도해 더 이상 움직일 수 없다면 GAME OVER.
            Vector2[] activePos = GetActivePosition();
            if (Background.Instance.IsCollision(activePos))
            {
                // GAME OVER.
                IsActivate = false;
            }
        }
    }

    
    static class BlockHandler
    {        
        static SHAPE nextShape;
        static Shape[] shapeData;                
        static BlockHandler()
        {
            shapeData = new Shape[7];
            shapeData[0] = new Shape()
            {
                shape = SHAPE.O,
                color = ConsoleColor.Blue,
                rotation = 0,
                datas = new short[,]{
                     { 
                        0, 0, 0, 0,
                        0, 1, 1, 0,
                        0, 1, 1, 0,
                        0, 0, 0, 0 
                    }
                }
            };
            shapeData[1] = new Shape()
            {
                shape = SHAPE.S,
                color = ConsoleColor.Cyan,
                rotation = 0,
                datas = new short[,]{
                     { 
                        0, 0, 0, 0,
                        0, 0, 1, 1,
                        0, 1, 1, 0,
                        0, 0, 0, 0 
                    },
                     { 
                        0, 0, 1, 0,
                        0, 0, 1, 1,
                        0, 0, 0, 1,
                        0, 0, 0, 0 
                    }
                }
            };
            shapeData[2] = new Shape()
            {
                shape = SHAPE.Z,
                color = ConsoleColor.DarkYellow,
                rotation = 0,
                datas = new short[,]{
                     {
                        0, 0, 0, 0,
                        0, 1, 1, 0,
                        0, 0, 1, 1,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 0, 1,
                        0, 0, 1, 1,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    }
                }
            };
            shapeData[3] = new Shape()
            {
                shape = SHAPE.J,
                color = ConsoleColor.Green,
                rotation = 0,
                datas = new short[,]{
                     {
                        0, 0, 0, 0,
                        0, 1, 1, 1,
                        0, 0, 0, 1,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 1,
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 1, 0, 0,
                        0, 1, 1, 1,
                        0, 0, 0, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 1, 1, 0,
                        0, 0, 0, 0
                    }
                }
            };
            shapeData[4] = new Shape()
            {
                shape = SHAPE.L,
                color = ConsoleColor.Magenta,
                rotation = 0,
                datas = new short[,]{
                     {
                        0, 0, 0, 0,
                        0, 1, 1, 1,
                        0, 1, 0, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 1, 1,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 0, 1,
                        0, 1, 1, 1,
                        0, 0, 0, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 1, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    }
                }
            };
            shapeData[5] = new Shape()
            {
                shape = SHAPE.I,
                color = ConsoleColor.Red,
                rotation = 0,
                datas = new short[,]{
                     {
                        0, 0, 0, 0,
                        1, 1, 1, 1,
                        0, 0, 0, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 1, 0
                    }
                }
            };
            shapeData[6] = new Shape()
            {
                shape = SHAPE.T,
                color = ConsoleColor.Yellow,
                rotation = 0,
                datas = new short[,]{
                     {
                        0, 0, 0, 0,
                        0, 1, 1, 1,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 0, 1, 1,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 1, 1, 1,
                        0, 0, 0, 0,
                        0, 0, 0, 0
                    },
                     {
                        0, 0, 1, 0,
                        0, 1, 1, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 0
                    }
                }
            };

            nextShape = (SHAPE)new Random().Next(0, (int)SHAPE.COUNT);
        }

        public static Shape GetShape(SHAPE type)
        {
            Shape shape = shapeData.FirstOrDefault(s => s.shape == type);
            shape.rotation = 0;
            return shape;
        }
        public static Shape GetRandomShape()
        {
            // 미리 정해진 다음 모양 데이터를 리턴하고 새로운 모양을 대입한다.
            Shape shape = shapeData[(int)nextShape];
            shape.rotation = 0;
            nextShape = (SHAPE)new Random().Next(0, (int)SHAPE.COUNT);
            return shape;
        }
        public static Shape GetNextShape()
        {
            // 미리 정해진 다음 모양 데이터를 리턴하고 새로운 모양을 대입한다.
            return shapeData[(int)nextShape];
        }

        public static void MoveVector(Vector2[] positions, DIRECTION dir, int distance = 1)
        {
            // dir방향에 따라 위치값을 변화시킨다.
            Vector2 movement = dir switch
            {
                DIRECTION.UP => new Vector2(0, -1),
                DIRECTION.DOWN => new Vector2(0, 1),
                DIRECTION.LEFT => new Vector2(-1, 0),
                DIRECTION.RIGHT => new Vector2(1, 0),
                _ => Vector2.Zero
            };

            // 모든 Dot의 위치가 movement만큼 이동.
            for (int i = 0; i < positions.Length; i++)
                positions[i] += movement * distance;
        }
    }
}
