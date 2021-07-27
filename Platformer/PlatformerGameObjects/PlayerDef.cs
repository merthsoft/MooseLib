﻿namespace Platformer.PlatformerGameObjects
{
    record PlayerDef : PlatformerGameObjectDef
    {
        public PlayerDef() : base("player", "Mush")
        {
            IsEffectedByGravity = true;
        }
    }
}
