using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MooseLib;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    class Snowball : GameObject
    {
        private class States
        {
            public const string Fly = "fly";
            public const string Hit = "hit";
        }
        
        public Queue<Vector2> FlightPath { get; } = new Queue<Vector2>();

        public Snowball(MooseGame parentGame, SpriteSheet spriteSheet, Vector2 startPosition, IEnumerable<Vector2> flightPath) 
            : base(parentGame, spriteSheet, startPosition, new(0f, 0f), state: States.Fly, layer: SnowballFightGame.SnowballLayer) 
        {
            FlightPath = new(flightPath.Where((v, i) => i % 5 == 0));
        }

        public override void Update(GameTime gameTime)
        {

            if (State == States.Fly)
            {
                Position = FlightPath.Dequeue();
                if (FlightPath.Count == 0)
                    State = States.Hit;
            }

            base.Update(gameTime);
        }
    }
}
