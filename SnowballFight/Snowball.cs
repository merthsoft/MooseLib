using Microsoft.Xna.Framework;
using MooseLib;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    class Snowball : GameObject
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

        public Snowball(MooseGame parentGame, Vector2 startPosition, IEnumerable<Vector2> flightPath) 
            : base(parentGame, AnimationKey, startPosition, spriteOffset, state: States.Fly, layer: SnowballFightGame.SnowballLayer) 
        {
            FlightPath = new(flightPath.Where((v, i) => i % 3 == 0));
            if (FlightPath.Count == 0)
                State = States.Dead;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == States.Fly)
            {
                Position = FlightPath.Dequeue();
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
