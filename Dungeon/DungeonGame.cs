using Merthsoft.Moose.MooseEngine;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    public DungeonGame()
    {
    }

    protected override void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        var productVersion = fileVersionInfo.ProductVersion!.Split('+')[0];

        Window.Title += $"Dungeon - v{productVersion}";
    }
}
