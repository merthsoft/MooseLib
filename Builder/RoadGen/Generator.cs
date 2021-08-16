using Roy_T.AStar.Graphs;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;

namespace Merthsoft.BusRl.RoadGen
{
    public static class Generator
    {
        private static Random Random { get; set; } = new();

        public static void SeedRandom(int seed)
            => Random = new Random(seed);

        public static int Next(int minValue, int maxValue)
            => Random.Next(minValue, maxValue);

        public static int Next(int maxValue)
            => Random.Next(maxValue);

        public static double Next()
            => Random.NextDouble();

        public static double Next(double minValue, double maxValue)
            => maxValue * Random.NextDouble() + minValue;

        public static int[,] Generate(int width, int height, int maxGenerations, IEnumerable<Crawler> seedCrawlers)
        {
            var nodes = new Node[width, height];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    nodes[i, j] = new(new(i, j));

            var grid = Grid.CreateGridFrom2DArrayOfNodes(nodes);

            var velocity = Velocity.FromMetersPerSecond(1);

            Stack<Crawler> crawlers = new();
            
            foreach (var seedCrawler in seedCrawlers)
                crawlers.Push(seedCrawler);

            int step = 0;
            while (crawlers.Count > 0 || step == 10_000_000)
            {
                var crawler = crawlers.Pop();
                var proceed = crawler.Crawl();
                var next = crawler.Emit();

                if (crawler.IsInBounds() && (crawler.X != crawler.OldX || crawler.Y != crawler.OldY))
                {
                    var fromNode = nodes[crawler.OldX, crawler.OldY];
                    var toNode = nodes[crawler.X, crawler.Y];

                    fromNode.Connect(toNode, velocity);
                    toNode.Connect(fromNode, velocity);
                }

                if (proceed)
                {
                    if (next != null && next.IsInBounds() && next.Generation < maxGenerations)
                        crawlers.Push(next);

                    if (crawler.IsInBounds())
                        crawlers.Push(crawler);
                }

                step++;
            }

            var result = new int[width, height];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    result[i, j] = (nodes[i, j]?.Outgoing?.Count ?? 0) > 0 ? 1 : 0;

            return result;
        }
    }
}
