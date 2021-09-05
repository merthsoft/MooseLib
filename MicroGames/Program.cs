using System;

namespace Merthsoft.Moose.MicroGames
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new GameManager();
            game.Run();
        }
    }
}
