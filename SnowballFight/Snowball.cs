﻿using Microsoft.Xna.Framework;
using MooseLib.Defs;
using MooseLib.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    internal class Snowball : AnimatedGameObject
    {
        public class States
        {
            public const string Fly = "fly";
            public const string Hit = "hit";
            public const string Dead = "dead";
        }

        public const string AnimationKey = "snowball";

        private Queue<Vector2> FlightPath { get; }
        private Vector2 StartCell { get; }

        public Snowball(AnimatedGameObjectDef def, IEnumerable<Vector2> flightPath) 
            : base(def, flightPath.First(), SnowballFightGame.SnowballLayer, state: States.Fly)
        {
            FlightPath = new(flightPath.Where((v, i) => i % 3 == 0));

            if (FlightPath.Count == 0)
                State = States.Dead;

            SpriteTransform = new(new(-8, -8), 0, Vector2.One);
            StartCell = GetCell();
        }

        public override void Update(GameTime gameTime)
        {
            if (State == States.Fly)
            {
                WorldPosition = FlightPath.Dequeue();
                if (IsBlocked())
                {
                    State = States.Hit;
                    StateCompleteAction = () => State = States.Dead;
                }
            }

            if (State == States.Dead)
                RemoveFlag = true;

            base.Update(gameTime);
        }

        private bool IsBlocked()
            => FlightPath.Count == 0
            || (GetCell(ParentMap!) != StartCell 
                && ParentMap.GetBlockingMap(WorldPosition).Skip(2).Take(3).Any(b => b != 0));
    }
}
