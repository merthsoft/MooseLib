using System;

namespace Merthsoft.Moose.Builder
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new BuilderGame())
                game.Run();
        }
    }
}
