namespace Merthsoft.Moose.Cats;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using var game = new CatGame();
        game.Run();
    }
}
