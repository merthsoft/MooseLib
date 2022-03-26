using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Cats;

public class CatGame : MooseGame
{
    public CatGame()
    {
    }

    protected override void Load()
    {
        AddDef(new CatDef("black"));
        AddDef(new CatDef("blue"));
        AddDef(new CatDef("brown"));
        AddDef(new CatDef("creme"));
        AddDef(new CatDef("dark"));
        AddDef(new CatDef("green"));
        AddDef(new CatDef("grey"));
        AddDef(new CatDef("grey_tabby"));
        AddDef(new CatDef("orange"));
        AddDef(new CatDef("orange_tabby"));
        AddDef(new CatDef("pink"));
        AddDef(new CatDef("radioactive"));
        AddDef(new CatDef("Seal_point"));
        AddDef(new CatDef("white"));
        AddDef(new CatDef("white_grey"));
        AddDef(new CatDef("black_gold_eyes"));

        ActiveMaps.Add(new CatMap());

        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        MainCamera.ZoomIn(1);
    }

    protected override void PostLoad()
    {
        foreach (var catdef in GetDefs<CatDef>())
            AddObject(new Cat(catdef, new(Random.Next(350), Random.Next(350))));
    }
}
