using System.Drawing;

namespace OSMStaticMap.Models
{
    internal class ClippedRectangleModel
    {
        public RectangleF ClippedRectangle { get; set; }
        public List<PinPosition> PinPositions { get; set; } = new List<PinPosition>();
    }
}
