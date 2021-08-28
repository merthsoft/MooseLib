using System;

namespace Merthsoft.Islands
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new IslandGame())
                game.Run();
        }
    }
}
