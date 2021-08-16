using System;

namespace Merthsoft.BusRl.RoadGen
{
    public class DirectionalCrawler : Crawler
    {
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public double Kinkiness { get; }

        private int x1;
        private int y1;
        private int x2;
        private int y2;
        private int deltaX;
        private int deltaY;
        private int stepX;
        private int stepY;
        private int err;

        public DirectionalCrawler(int generation, int x, int y, int width, int height,
            int destinationX, int destinationY, double kinkiness) : base(generation, x, y, width, height)
        {
            DestinationX = destinationX;
            DestinationY = destinationY;
            Kinkiness = kinkiness;

            (x1, y1) = (DestinationX, DestinationY);
            (x2, y2) = (X, Y);

            deltaX = Math.Abs(x1 - x2);
            deltaY = Math.Abs(y1 - y2);

            stepX = x2 < x1 ? 1 : -1;
            stepY = y2 < y1 ? 1 : -1;

            err = deltaX - deltaY;
        }

        protected override bool CrawlImp()
        {
            var e2 = 2 * err;

            if (e2 > -deltaY)
            {
                err -= deltaY;
                x2 += stepX;
            }

            if ((x2 != x1 || y2 != y1) && e2 < deltaX)
            {
                err += deltaX;
                y2 += stepY;
            }

            (X, Y) = (x2, y2);

            return (X, Y) != (DestinationX,  DestinationY);
        }

        public override Crawler? Emit()
        {
            if (Generator.Next() < Kinkiness)
            {
                int destinationX;
                int destinationY;
                var plane = Generator.Next();
                
                switch (Generation)
                {
                    case 0:
                        destinationX = plane <= .5 ? Generator.Next(5, Width - 5) : X;
                        destinationY = plane >= .5 ? Generator.Next(5, Height - 5) : Y;
                        break;
                    case 1:
                    case 2:
                        destinationX = plane <= .5 ? X + Generator.Next(-6, 7) : X;
                        destinationY = plane >= .5 ? Y + Generator.Next(-6, 7) : Y;
                        break;
                    case 3:
                        destinationX = X + Generator.Next(-3, 4);
                        destinationY = Y + Generator.Next(-3, 4);
                        break;
                    default:
                        destinationX = X + Generator.Next(-2, 3);
                        destinationY = Y + Generator.Next(-2, 3);
                        break;
                }
                return new DirectionalCrawler(Generation + 1, X, Y, Width, Height, destinationX, destinationY, Kinkiness - .1);
            }

            return null;
        }
    }
}
