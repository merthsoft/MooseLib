using System;

namespace SnowballFight
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new SnowballFightGame();
            game.Run();
        }
    }
}
