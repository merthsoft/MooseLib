using System;

namespace Merthsoft.BusRl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new BusRl();
            game.Run();
        }
    }
}
