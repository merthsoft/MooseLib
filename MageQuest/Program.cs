using System;

namespace Merthsoft.Moose.MageQuest
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MageGame())
                game.Run();
        }
    }
}
