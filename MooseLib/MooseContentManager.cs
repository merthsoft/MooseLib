using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System.Collections.Generic;

namespace Merthsoft.MooseEngine
{
    public class MooseContentManager
    {
        ContentManager Content { get; set; }

        public readonly Dictionary<string, SpriteSheet> AnimationSpriteSheets = new();

        public MooseContentManager(ContentManager content)
        {
            Content = content;
            Content.RootDirectory = nameof(Content);
        }

        public SpriteSheet LoadAnimatedSpriteSheet(string animationKey, bool replace = false)
        {
            if (replace || !AnimationSpriteSheets.ContainsKey(animationKey))
                AnimationSpriteSheets[animationKey] = Content.Load<SpriteSheet>($"Animations/{animationKey}.sf", new JsonContentLoader());

            return AnimationSpriteSheets[animationKey];
        }
    }
}
