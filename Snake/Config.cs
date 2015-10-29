using System;
using System.IO;

namespace Snake
{
    public static class Config
    {
        static Config()
        {
            Reload();
        }
        public static void Reload()
        {
            // TODO: Load from config file.


        }

        public static int EntitySize { get; } = 32;
        public static int EntityCountPerAxis { get; set; } = 16;

        public static int WindowWidth { get; set; } = EntitySize * EntityCountPerAxis;
        public static int WindowHeight { get; set; } = WindowWidth;

        public static string HighScoreFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "!Snake", "HighScore.save");

        public static bool WallsActive { get; set; } = true;
    }
}