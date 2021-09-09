using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using SpriteFontPlus;
using System.Collections.Generic;
using System.IO;

namespace Merthsoft.Moose.MooseEngine
{
    public class MooseContentManager
    {
        ContentManager Content { get; }
        GraphicsDevice GraphicsDevice {  get;}

        public readonly Dictionary<string, SpriteSheet> AnimationSpriteSheets = new();

        public MooseContentManager(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;
            GraphicsDevice = graphicsDevice;
            Content.RootDirectory = nameof(Content);
        }

        public SpriteSheet LoadAnimatedSpriteSheet(string animationKey, bool replace = false)
        {
            if (replace || !AnimationSpriteSheets.ContainsKey(animationKey))
                AnimationSpriteSheets[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

            return AnimationSpriteSheets[animationKey];
        }

        private static readonly CharacterRange[] DefaultCharacterRange = new[]
        {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic
        };

        private static readonly int DefaultBitmapSize = 1024;

        public SpriteFont BakeFont(string font, int fontPixelHeight)
            => TtfFontBaker.Bake(
                    File.ReadAllBytes($"Content/Fonts/{font}.ttf"), fontPixelHeight, 
                    DefaultBitmapSize, DefaultBitmapSize, DefaultCharacterRange
               ).CreateSpriteFont(GraphicsDevice);
    }
}
