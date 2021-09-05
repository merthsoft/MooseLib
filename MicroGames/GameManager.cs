﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System.Collections.Generic;
using System.IO;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Ui;

namespace Merthsoft.Moose.MicroGames
{
    public class GameManager : MooseGame
    {
        private readonly List<MooseGame> Games = new();
        private WindowManager WindowManager = null!;

        public GameManager() { }

        protected override StartupParameters Startup()
            => new()
            {
                ScreenHeight = 768,
                ScreenWidth = 1024,
            };

        protected override void Load()
        {
            var fonts = new[]
            {
                ContentManager.BakeFont("Outward_Bound", 78),
                ContentManager.BakeFont("Tomorrow_Night", 78),
            };

            var windowTextures = new[] {
                Content.Load<Texture2D>("Images/wooden_window"),
                Content.Load<Texture2D>("Images/fancy_window"),
            };

            var themes = new Theme[] {
                new("default", windowTextures[0], 16, 16, fonts) 
                { 
                    ControlDrawOffset = new(5, 5), 
                    TextColor = Color.White, 
                    TextMouseOverColor = Color.Maroon,
                    TileScale = new(7, 7),
                },
                new("fancy", windowTextures[1], 16, 16, fonts) 
                { 
                    ControlDrawOffset = new(7, 7), 
                    TextColor = Color.White, 
                    TextMouseOverColor = Color.Maroon,
                    TileScale = new(7, 7),
                }, 
            };

            WindowManager = new WindowManager(themes);
            var titleWindow = WindowManager.NewWindow(0, 0, 384, 128, "fancy");
            titleWindow.AddLabel(21, 20, "Games!", 1);
            titleWindow.Center(ScreenWidth, titleWindow.Height);

            DefaultBackgroundColor = new(36, 34, 52);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            WindowManager.Update(gameTime, CurrentMouseState);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(SpriteBatch);
            SpriteBatch.End();
        }
    }
}
