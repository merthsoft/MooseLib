using System;

namespace CyberpunkRl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Cprl();
            game.Run();
        }
    }
}
