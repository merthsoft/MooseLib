using System;
using Merthsoft.Moose.Town;

namespace Merthsoft.Moose.Town
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TownGame())
                game.Run();
        }
    }
}
