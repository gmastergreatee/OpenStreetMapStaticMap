using System.Drawing;

namespace OSMStaticMap.Models
{
    internal class PinPosition
    {
        public PointF Point { get; set; }
        public PointF LabelPoint { get; set; }
        public PinLabelPositionEnum LabelPosition { get; set; }
        public PointF PinPositionOffset { get; set; }
        public string Label { get; set; } = "";
    }
}
