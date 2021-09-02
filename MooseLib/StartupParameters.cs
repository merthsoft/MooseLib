﻿using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine
{
    public class StartupParameters
    {
        public BlendState? BlendState { get; set; }
        public SamplerState? SamplerState { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public bool Fullscreen { get; set; }
        public int? RandomSeed { get; set; }
    }
}