using System;

namespace Snake
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SnakeGame())
                game.Run();
        }
    }
#endif
}
