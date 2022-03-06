namespace Merthsoft.Moose.Dungeon;

record DungeonPlayerDef : DungeonObjectDef
{
    public Texture2D Texture { get; private set; } = null!; // Loaded in LoadContent

    public DungeonPlayerDef() : base("player") { }

    public override void LoadContent(MooseContentManager contentManager)
        => Texture = contentManager.LoadImage("Heroes");
}
