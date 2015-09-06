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

        public static int WindowWidth { get; set; } = 800;
        public static int WindowHeight { get; set; } = WindowWidth / 4 * 3;

        public static int EntityCountOnX { get; set; } = 25;
        public static int EntitySize { get; } = WindowWidth / EntityCountOnX;

        public static bool WallsActive { get; set; } = false;
    }
}