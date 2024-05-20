using System;

namespace Merthsoft.Moose.Island
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new IslandGame();
            game.Run();
        }
    }
}
