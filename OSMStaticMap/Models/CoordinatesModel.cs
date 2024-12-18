using System.Drawing;

namespace OSMStaticMap.Models
{
    public class CoordinatesModel
    {
        public float LatitudeDegrees { get; set; }
        public float LongitudeDegrees { get; set; }

        public bool ShowPin { get; set; } = false;

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
