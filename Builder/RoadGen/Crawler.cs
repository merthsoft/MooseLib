namespace Merthsoft.BusRl.RoadGen
{
    public abstract class Crawler
    {
        public int Generation { get; }
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }

        public int OldX { get; protected set; }
        public int OldY { get; protected set; }

        public int Width { get; }
        public int Height { get; }

        public Crawler(int generation, int x, int y, int width, int height)
        {
            Generation = generation;
            X = x;
            Y = y;
            OldX = x;
            OldY = y;
            Width = width;
            Height = height;
        }

        public bool IsInBounds()
            => X >= 0 && X < Width
            && Y >= 0 && Y < Height;

        public bool IsInBounds(int x, int y)
            => x >= 0 && x <= Width
            && y >= 0 && y <= Height;

        public bool Crawl()
        {
            OldX = X;
            OldY = Y;

            return CrawlImp();
        }

        protected abstract bool CrawlImp();
        
        public abstract Crawler? Emit();
    }
}
