﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using SpriteFontPlus;

namespace Merthsoft.Moose.MooseEngine
{
    public class MooseContentManager
    {
        ContentManager Content { get; }
        
        public MooseGame Game { get; }
        public GraphicsDevice GraphicsDevice {  get;}

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
