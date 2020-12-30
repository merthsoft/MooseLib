using Microsoft.Xna.Framework;
using MooseLib.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    class Snowball : AnimatedGameObject
    {
        public const string AnimationKey = "snowball";
        private static readonly Vector2 spriteOffset = new(4f, 4f);

        private class States
        {
            public const string Fly = "fly";
            public const string Hit = "hit";
            public const string Dead = "dead";
        }
        
        public Queue<Vector2> FlightPath { get; } = new Queue<Vector2>();

        public Snowball(SnowballFightGame parentGame, Vector2 startPosition, IEnumerable<Vector2> flightPath) 
            : base(parentGame, AnimationKey, startPosition, spriteOffset, state: States.Fly, layer: parentGame.SnowballLayer) 
        {
            FlightPath = new(flightPath.Where((v, i) => i % 3 == 0));
            if (FlightPath.Count == 0)
                State = States.Dead;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == States.Fly)
            {
                WorldPosition = FlightPath.Dequeue();
                if (FlightPath.Count == 0)
                {
                    State = States.Hit;
                    StateCompleteAction = () => State = States.Dead;
                }
            }

            if (State == States.Dead)
                RemoveFlag = true;

            base.Update(gameTime);
        }
    }
}
