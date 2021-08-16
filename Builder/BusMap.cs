using Merthsoft.MooseEngine.BaseDriver;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.BusRl
{
    class BusMap : TileMap
    {
        public BusMap(int width, int height, int numLayers, int tileWidth, int tileHeight) 
            : base(width, height, numLayers, tileWidth, tileHeight) { }

        public override void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
        {
            
        }

        protected override int IsBlockedAt(int layer, int x, int y)
        {
            return 0;
        }
    }
}
