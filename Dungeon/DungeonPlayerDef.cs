namespace Merthsoft.Moose.Dungeon;

public record DungeonPlayerDef : DungeonObjectDef
{
    public Texture2D Texture { get; private set; } = null!; // Loaded in LoadContent

    public DungeonPlayerDef() : base("player") {
        DefaultLayer = "player";
    }

    public override void LoadContent(MooseContentManager contentManager)
        => Texture = contentManager.LoadImage("Heroes");
}
