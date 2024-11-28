using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStreetMapStaticMaps
{
    internal class LatLonTileObject
    {
        public PointF LatLon { get; set; }
        public PointF TileXY { get; set; }
    }
}
