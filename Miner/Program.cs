using System;

namespace Merthsoft.Moose.Miner
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MinerGame())
                game.Run();
        }
    }
}
