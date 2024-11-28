namespace OpenStreetMapStaticMaps.Core.Models
{
    internal class MapModel
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Image? MapImage { get; set; }
    }
}
