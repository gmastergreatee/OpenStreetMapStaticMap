using System.Drawing;

namespace OSMStaticMap.Models
{
    public class TileModel
    {
        public Point RenderOffset { get; set; }

        public Point TileCoords { get; set; }

        public Image? TileImage { get; set; }

        public TileModel(PointF? coordinates = null)
        {
            if (coordinates != null)
            {
                this.TileCoords = new Point(
                    Convert.ToInt32(Math.Floor(coordinates.Value.X)),
                    Convert.ToInt32(Math.Floor(coordinates.Value.Y))
                );
            }
        }
    }
}
