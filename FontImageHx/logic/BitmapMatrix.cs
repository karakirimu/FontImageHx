namespace FontImageHx
{
    internal static class BitmapMatrix
    {
        public static void Move(byte[][]? bitmap, MoveDirection direction)
        {
            if (bitmap == null)
            {
                return;
            }

            switch (direction)
            {
                case MoveDirection.Up:
                    for (int y = 1; y < bitmap.Length; y++)
                    {
                        int ty = y - 1;
                        int ny = y;

                        for (int x = 0; x < bitmap[0].Length; x++)
                        {
                            bitmap[ty][x] = bitmap[ny][x];

                            if (bitmap.Length - 1 == ny)
                            {
                                bitmap[ny][x] = 0;
                            }
                        }
                    }
                    break;
                case MoveDirection.Down:
                    for (int y = 1; y < bitmap.Length; y++)
                    {
                        int ty = bitmap.Length - y;
                        int ny = bitmap.Length - 1 - y;

                        for (int x = 0; x < bitmap[0].Length; x++)
                        {
                            bitmap[ty][x] = bitmap[ny][x];

                            if (0 == ny)
                            {
                                bitmap[ny][x] = 0;
                            }
                        }
                    }
                    break;
                case MoveDirection.Left:
                    for (int y = 0; y < bitmap.Length; y++)
                    {
                        for (int x = 1; x < bitmap[0].Length; x++)
                        {
                            int tx = x - 1;
                            int nx = x;

                            bitmap[y][tx] = bitmap[y][nx];

                            if (bitmap[0].Length - 1 == nx)
                            {
                                bitmap[y][nx] = 0;
                            }
                        }
                    }
                    break;
                case MoveDirection.Right:
                    for (int y = 0; y < bitmap.Length; y++)
                    {
                        for (int x = 1; x < bitmap[0].Length; x++)
                        {
                            int tx = bitmap[0].Length - x;
                            int nx = bitmap[0].Length - 1 - x;

                            bitmap[y][tx] = bitmap[y][nx];

                            if (0 == nx)
                            {
                                bitmap[y][nx] = 0;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
