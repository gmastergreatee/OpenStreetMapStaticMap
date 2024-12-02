using System.Drawing;

namespace OSMStaticMap.Models
{
    internal class ClippedRectangleModel
    {
        public RectangleF ClippedRectangle { get; set; }
        public List<PointF> PinPositions { get; set; } = new List<PointF>();
    }
}
