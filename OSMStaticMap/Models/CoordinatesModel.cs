using System.Drawing;

namespace OSMStaticMap.Models
{
    public class CoordinatesModel
    {
        public float LatitudeDegrees { get; set; }
        public float LongitudeDegrees { get; set; }

        public bool ShowPin { get; set; } = false;
        public string PinLabel { get; set; } = "";
        public PinLabelPositionEnum PinPosition { get; set; } = PinLabelPositionEnum.Center;
        public PointF PinPositionOffset { get; set; } = new PointF(0, 0);

        public static CoordinatesModel FromPointF(PointF point)
        {
            return new CoordinatesModel
            {
                LatitudeDegrees = point.X,
                LongitudeDegrees = point.Y,
                ShowPin = true
            };
        }
    }
}
