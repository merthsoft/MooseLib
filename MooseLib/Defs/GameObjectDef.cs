namespace Merthsoft.Moose.MooseEngine.Defs;

public record GameObjectDef : Def
{
    public string DefaultLayer { get; set; } = "";
    public Vector2 DefaultSize { get; set; } = Vector2.One;
    public Vector2 DefaultPosition { get; set; }
    public Vector2 DefaultScale { get; set; } = Vector2.One;
    public Vector2 DefaultOrigin { get; set; }
    public float DefaultRotation { get; set; }

    public int WorldSizeRound { get; set; } = 2;

    public GameObjectDef(string defName) : base(defName) { }
}
