using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MooseLib.GameObjects;

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
        private static readonly Vector2 spriteOffset = new(4f, 4f);

        public Queue<Vector2> FlightPath { get; } = new Queue<Vector2>();

        public Snowball(SnowballFightGame parentGame, Vector2 startPosition, IEnumerable<Vector2> flightPath) 
            : base(parentGame, AnimationKey, startPosition, States.Fly, parentGame.SnowballLayer, scale: new(.75f))
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
