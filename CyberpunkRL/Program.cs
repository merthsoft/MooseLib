namespace Merthsoft.Moose.CyberpunkRL;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        using var game = new Cprl();
        game.Run();
    }
}
