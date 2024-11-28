using System.Diagnostics;
using System.Drawing;
using System.Net;

namespace OpenStreetMapStaticMaps
{
    public partial class Form1 : Form
    {
        private List<PointF> pinList = new List<PointF>();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDrawIt_Click(object sender, EventArgs e)
        {
            btnDrawIt.Enabled = false;
            var zoom = Convert.ToInt32(txtZoom.Text);
            CalculatePins();

            if (pinList.Count > 0)
            {
                var pinImage = Bitmap.FromFile("./spotlight_pin_v4_dot-2-medium.png");
                SetImage(zoom, pinList, pinImage);
            }
            else
            {
                MessageBox.Show("No pins to mark!");
                btnDrawIt.Enabled = true;
            }
        }

        private void CalculatePins()
        {
            pinList.Clear();

            try
            {
                if (txtLon1.Text.Length > 0 && txtLat1.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat1.Text),
                        Convert.ToSingle(txtLon1.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon2.Text.Length > 0 && txtLat2.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat2.Text),
                        Convert.ToSingle(txtLon2.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon3.Text.Length > 0 && txtLat3.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat3.Text),
                        Convert.ToSingle(txtLon3.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon4.Text.Length > 0 && txtLat4.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat4.Text),
                        Convert.ToSingle(txtLon4.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon5.Text.Length > 0 && txtLat5.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat5.Text),
                        Convert.ToSingle(txtLon5.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon6.Text.Length > 0 && txtLat6.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat6.Text),
                        Convert.ToSingle(txtLon6.Text)
                    ));
                }
            }
            catch { }

            try
            {
                if (txtLon7.Text.Length > 0 && txtLat7.Text.Length > 0)
                {
                    pinList.Add(new PointF(
                        Convert.ToSingle(txtLat7.Text),
                        Convert.ToSingle(txtLon7.Text)
                    ));
                }
            }
            catch { }
        }

        public async void SetImage(int zoom, List<PointF> pins, Image pinImage)
        {
            picMapImage.Image = await GetFullImage(pinList, 456, 343, pinImage);
            MessageBox.Show("Done!");
            btnDrawIt.Enabled = true;
        }

        public async Task<Image> GetFullImage(List<PointF> pins, int mapWidthInPixels, int mapHeightInPixels, Image pinImage)
        {
            var minLat = pins.Min(i => i.X);
            var minLong = pins.Min(i => i.Y);
            var maxLat = pins.Max(i => i.X);
            var maxLong = pins.Max(i => i.Y);

            var zoom = CalculateZoomLevel(minLat, maxLat, minLong, maxLong, mapWidthInPixels, mapHeightInPixels);

            var maxTileCountPerAxis = Math.Pow(2, zoom);
            var tileMappingList = new List<LatLonTileObject>();

            foreach (var pin in pins)
            {
                tileMappingList.Add(new LatLonTileObject()
                {
                    LatLon = pin,
                    TileXY = TileObject.WorldToTilePos(pin.Y, pin.X, zoom)
                });
            }

            var fromTileX = Convert.ToInt32(tileMappingList.Min(i => i.TileXY.X));
            var toTileX = Convert.ToInt32(tileMappingList.Max(i => i.TileXY.X));

            var fromTileY = Convert.ToInt32(tileMappingList.Min(i => i.TileXY.Y));
            var toTileY = Convert.ToInt32(tileMappingList.Max(i => i.TileXY.Y));

            #region Extend map boundary by 1 tile on all sides
            if (fromTileX > 0)
            {
                fromTileX -= 1;
            }

            if (toTileX < maxTileCountPerAxis)
            {
                toTileX += 1;
            }

            if (fromTileY > 0)
            {
                fromTileY -= 1;
            }

            if (toTileY < maxTileCountPerAxis)
            {
                toTileY += 1;
            }
            #endregion

            var tileList = new List<TileObject>();
            for (int tX = fromTileX, x_offset = 0; tX <= toTileX; tX++, x_offset++)
            {
                for (int tY = fromTileY, y_offset = 0; tY <= toTileY; tY++, y_offset++)
                {
                    tileList.Add(new TileObject()
                    {
                        X_Offset = x_offset,
                        Y_Offset = y_offset,
                        X_Tile = tX,
                        Y_Tile = tY,
                    });
                }
            }

            await Parallel.ForEachAsync(tileList, async (tileObj, cancellationToken) =>
            {
                tileObj.TileImage = await GetMapTile(zoom, tileObj.X_Tile, tileObj.Y_Tile);
            });

            var bitMap = new Bitmap(256 * (toTileX - fromTileX + 1), 256 * (toTileY - fromTileY + 1));
            var g = Graphics.FromImage(bitMap);

            // draw map tiles
            foreach (var tile in tileList)
            {
                g.DrawImage(tile.TileImage, new Point(256 * tile.X_Offset, 256 * tile.Y_Offset));
            }

            var pinPositionsInImage = new List<PointF>();

            // draw pins
            if (pins != null)
            {
                foreach (var pin in pins.OrderBy(i => i.X).ThenBy(i => i.Y))
                {
                    var pinTilePos = TileObject.WorldToTilePos(pin.Y, pin.X, zoom);

                    var tile = tileList.FirstOrDefault(i =>
                        i.X_Tile <= pinTilePos.X && i.X_Tile + 1 > pinTilePos.X &&
                        i.Y_Tile <= pinTilePos.Y && i.Y_Tile + 1 > pinTilePos.Y
                    );

                    if (tile != null)
                    {
                        var x_in_tile = 256 * (pinTilePos.X - Math.Round(pinTilePos.X, MidpointRounding.ToZero));
                        var y_in_tile = 256 * (pinTilePos.Y - Math.Round(pinTilePos.Y, MidpointRounding.ToZero));

                        var pinPosInImage = new PointF(
                            Convert.ToInt32((tile.X_Offset * 256) + x_in_tile),
                            Convert.ToInt32((tile.Y_Offset * 256) + y_in_tile)
                        );

                        g.DrawImage(
                            pinImage,
                            new PointF(
                                pinPosInImage.X - (pinImage.Width / 2),
                                pinPosInImage.Y - (pinImage.Height / 2)
                            )
                        );

                        pinPositionsInImage.Add(pinPosInImage);
                    }
                }
            }

            var medianX = pinPositionsInImage.Sum(i => i.X) / pinPositionsInImage.Count;
            var medianY = pinPositionsInImage.Sum(i => i.Y) / pinPositionsInImage.Count;

            var halfWidth = mapWidthInPixels / 2;
            var halfHeight = mapHeightInPixels / 2;

            var medianPosition = new PointF(
                Math.Max(0, medianX - halfWidth),
                Math.Max(0, medianY - halfHeight)
            );
            var clipRect = new RectangleF(medianPosition.X, medianPosition.Y, mapWidthInPixels, mapHeightInPixels);
            return bitMap.Clone(clipRect, bitMap.PixelFormat);
        }

        public static int CalculateZoomLevel(double minLat, double maxLat, double minLon, double maxLon, int mapWidth, int mapHeight)
        {
            const double tileSize = 256.0; // Tile size in pixels
            const int maxZoomLevel = 19;   // Maximum zoom level for OSM

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

        public async Task<Image> GetFullImage(int zoom, double longitude, double latitude, List<PointF> pins)
        {
            var pinImage = Bitmap.FromFile("./spotlight_pin_v4_dot-2-medium.png");

            var tilePos = TileObject.WorldToTilePos(longitude, latitude, zoom);
            var baseTileObject = new TileObject()
            {
                X_Offset = 1,
                Y_Offset = 1,
                X_Tile = Convert.ToInt32(tilePos.X),
                Y_Tile = Convert.ToInt32(tilePos.Y)
            };

            var tileList = new List<TileObject>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tileList.Add(new TileObject()
                    {
                        X_Offset = i + 1,
                        Y_Offset = j + 1,
                        X_Tile = baseTileObject.X_Tile + i,
                        Y_Tile = baseTileObject.Y_Tile + j,
                        TileImage = await GetMapTile(zoom, baseTileObject.X_Tile + i, baseTileObject.Y_Tile + j),
                    });
                }
            }

            var bitMap = new Bitmap(256 * 3, 256 * 3);
            var g = Graphics.FromImage(bitMap);
            foreach (var tile in tileList)
            {
                g.DrawImage(tile.TileImage, new Point(256 * tile.X_Offset, 256 * tile.Y_Offset));
            }

            if (pins != null)
            {
                foreach (var pin in pins)
                {
                    var pinTilePos = TileObject.WorldToTilePos(pin.Y, pin.X, zoom);
                    var tile = tileList.FirstOrDefault(i =>
                        i.X_Tile <= pinTilePos.X && i.X_Tile + 1 > pinTilePos.X &&
                        i.Y_Tile <= pinTilePos.Y && i.Y_Tile + 1 > pinTilePos.Y
                    );

                    if (tile != null)
                    {
                        var x_in_tile = 256 * (pinTilePos.X - Math.Round(pinTilePos.X, MidpointRounding.ToZero));
                        var y_in_tile = 256 * (pinTilePos.Y - Math.Round(pinTilePos.Y, MidpointRounding.ToZero));

                        g.DrawImage(
                            pinImage,
                            new Point(
                                Convert.ToInt32((tile.X_Offset * 256) + x_in_tile),
                                Convert.ToInt32((tile.Y_Offset * 256) + y_in_tile)
                            )
                        );
                    }
                }
            }
            return bitMap;
        }

        private async Task<Image> GetMapTile(int zoom, int x, int y)
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var address = new Uri($"https://tile.openstreetmap.org/{zoom}/{x}/{y}.png");
                    var req = new HttpRequestMessage(HttpMethod.Get, address);
                    req.Headers.UserAgent.Add(System.Net.Http.Headers.ProductInfoHeaderValue.Parse("Mozilla/5.0"));
                    req.Headers.UserAgent.Add(System.Net.Http.Headers.ProductInfoHeaderValue.Parse("AppleWebKit/537.36"));
                    req.Headers.UserAgent.Add(System.Net.Http.Headers.ProductInfoHeaderValue.Parse("Chrome/131.0.0.0"));
                    req.Headers.UserAgent.Add(System.Net.Http.Headers.ProductInfoHeaderValue.Parse("Safari/537.36"));
                    var response = await hc.SendAsync(req);
                    var imageStream = response.Content.ReadAsStream();
                    return Bitmap.FromStream(imageStream);
                }
            }
            catch
            {
                return new Bitmap(256, 256);
            }
        }
    }
}
