namespace OSMStaticMap.Models
{
    public class CoordinatesModel
    {
        public float LatitudeDegrees { get; set; }
        public float LongitudeDegrees { get; set; }

        public bool ShowPin { get; set; } = false;
    }
}
