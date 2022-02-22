namespace Merthsoft.Moose.AtmSurvivors;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new AtmGame())
            game.Run();
    }
}
