using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using SpriteFontPlus;

namespace Merthsoft.Moose.MooseEngine;

public class MooseContentManager
{
    private static readonly int DefaultBitmapSize = 1024;
    public static readonly CharacterRange BasicAsciiRange = new(' ', '~');

    public List<CharacterRange> DefaultCharacterRange { get; } = new() { BasicAsciiRange };

    ContentManager Content { get; }

    public MooseGame Game { get; }
    public GraphicsDevice GraphicsDevice { get; }

    public readonly Dictionary<string, SpriteSheet> AnimationSpriteSheets = new();

    public MooseContentManager(MooseGame game, ContentManager content, GraphicsDevice graphicsDevice)
    {
        Game = game;
        Content = content;
        GraphicsDevice = graphicsDevice;
        Content.RootDirectory = nameof(Content);
    }

    public TGame? GetGame<TGame>() where TGame : MooseGame
        => Game as TGame;

    public TContent Load<TContent>(string assetName)
        => Content.Load<TContent>(assetName);

    public Texture2D LoadImage(string assetName)
        => Content.Load<Texture2D>($"Images/{assetName}");

    public SpriteSheet LoadAnimatedSpriteSheet(string animationKey, bool replace = false)
    {
        if (replace || !AnimationSpriteSheets.ContainsKey(animationKey))
            AnimationSpriteSheets[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

        return AnimationSpriteSheets[animationKey];
    }

    public SpriteFont BakeFont(string font, int fontPixelHeight, CharacterRange[]? characterRange = null)
        => TtfFontBaker.Bake(
                File.ReadAllBytes($"Content/Fonts/{font}.ttf"), fontPixelHeight,
                DefaultBitmapSize, DefaultBitmapSize, characterRange ?? DefaultCharacterRange.ToArray()
           ).CreateSpriteFont(GraphicsDevice);
}
