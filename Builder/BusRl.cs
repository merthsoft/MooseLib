using Merthsoft.BusRl.RoadGen;
using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.BaseDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Merthsoft.BusRl
{
    public class BusRl : MooseGame
    {
        public BusRl()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            Graphics.PreferredBackBufferWidth = 1024;
            Graphics.PreferredBackBufferHeight = 768;
            Graphics.ApplyChanges();
        }

        protected override void Load()
        {
            AddRenderer(SpriteBatchObjectRenderer.DefaultRenderKey, new SpriteBatchObjectRenderer(SpriteBatch));
            AddRenderer(SpriteBatchPrimitiveRectangleRenderer.DefaultRenderKey, new SpriteBatchPrimitiveRectangleRenderer(SpriteBatch, 16, 16,
                Color.Green, Color.Black));

            MainMap = new BusMap(48, 48, 1, 16, 16);
            GenerateRoads();
        }

        private void GenerateRoads()
        {
            var layer = MainMap.Layers[0] as TileLayer<int>;

            var x1 = 1 + Generator.Next(3);
            var y1 = 1 + Generator.Next(3);

            var x2 = 1 + Generator.Next(3);
            var y2 = 44 + Generator.Next(3);

            var x3 = 44 + Generator.Next(3);
            var y3 = 44 + Generator.Next(3);

            var x4 = 44 + Generator.Next(3);
            var y4 = 1 + Generator.Next(3);

            const double Kinkiness = .4;
            var roads = Generator.Generate(48, 48, 6, new[] {
                new DirectionalCrawler(0, width: 48, height: 48,
                    x: x1, y: y1, destinationX: x2, destinationY: y2,
                    kinkiness: Kinkiness),
                new DirectionalCrawler(0, width: 48, height: 48,
                    x: x2, y: y2, destinationX: x3, destinationY: y3,
                    kinkiness: Kinkiness),
                new DirectionalCrawler(0, width: 48, height: 48,
                    x: x3, y: y3, destinationX: x4, destinationY: y4,
                    kinkiness: Kinkiness),
                new DirectionalCrawler(0, width: 48, height: 48,
                    x: x1, y: y1, destinationX: x4, destinationY: y4,
                    kinkiness: Kinkiness) });

            for (int i = 0; i < 48; i++)
                for (var j = 0; j < 48; j++)
                    layer!.Tiles[i, j] = roads[i, j];
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            if (WasKeyJustPressed(Keys.Space))
                GenerateRoads();
        }
    }
}
