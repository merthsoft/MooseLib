namespace Merthsoft.Moose.Rays;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using var game = new RayGame();
        game.Run();
    }
}
