namespace OpenStreetMapStaticMaps.Core.Helpers
{
    internal static class OSMMapHelper
    {
        public static int CalculateZoomLevel(double minLat, double maxLat, double minLon, double maxLon, int mapWidth, int mapHeight, double tileSize = 256, byte maxZoomLevel = 17)
        {
            // Calculate bounding box dimensions in degrees
            double boundingBoxWidth = maxLon - minLon;
            double boundingBoxHeight = maxLat - minLat;

            // Initialize zoom level
            double zoomLevel = 0;

            // Find the zoom level for the longitude dimension
            for (int z = 0; z <= maxZoomLevel; z++)
            {
                // Calculate the number of tiles required to cover the bounding box in longitude
                double longitudePerTile = 360.0 / Math.Pow(2, z);
                double tilesInLongitude = boundingBoxWidth / longitudePerTile;

                // Calculate the number of tiles required to cover the bounding box in latitude
                double latitudePerTile = 180.0 / Math.Pow(2, z);
                double tilesInLatitude = boundingBoxHeight / latitudePerTile;

                // Calculate how many tiles fit in the map dimensions
                double mapTilesWidth = mapWidth / tileSize;
                double mapTilesHeight = mapHeight / tileSize;

                // Check if the number of tiles in both dimensions fits the map size
                if (tilesInLongitude <= mapTilesWidth && tilesInLatitude <= mapTilesHeight)
                {
                    zoomLevel = z;  // This zoom level fits, so we select it
                }
                else
                {
                    break;  // Stop if we go beyond the required zoom level
                }
            }

            // Return the determined zoom level (clamped between 0 and 19)
            return (int)zoomLevel;
        }

        public static PointF WorldToTilePos(double lon, double lat, int zoom)
        {
            PointF p = new Point();
            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public static PointF TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            PointF p = new Point();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }
    }
}
