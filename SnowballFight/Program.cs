using System;

namespace Merthsoft.SnowballFight
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new SnowballFightGame();
            game.Run();
        }
    }
}
