using OpenStreetMapStaticMaps.Core.Helpers;
using OpenStreetMapStaticMaps.Core.Models;

namespace OpenStreetMapStaticMaps.Core
{
    internal class Mapper
    {
        public string TileURL { get; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ZoomLevel { get; }

        private int _zoom = 0;

        public Mapper(string URL, int imageWidth, int imageHeight, int zoom = 0)
        {
            this.TileURL = URL;
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            this.ZoomLevel = zoom > 0 ? Math.Min(zoom, 19) : 0;
        }

        public async Task<Image> PlotMapAsync(List<CoordinatesModel> coordinates, Image pinImage = null)
        {
            var minLatitude = coordinates.Min(i => i.LatitudeDegrees);
            var minLongitude = coordinates.Min(i => i.LongitudeDegrees);
            var maxLatitude = coordinates.Max(i => i.LatitudeDegrees);
            var maxLongitude = coordinates.Max(i => i.LongitudeDegrees);

            if (this.ZoomLevel <= 0)
            {
                // automatically determine zoom level
                this._zoom = OSMMapHelper.CalculateZoomLevel(
                    minLatitude,
                    maxLatitude,
                    minLongitude,
                    maxLongitude,
                    this.ImageWidth,
                    this.ImageHeight
                );
            }
            else
            {
                this._zoom = this.ZoomLevel;
            }

            var maxTileCountPerAxis = Math.Pow(2, this._zoom);
            var tileMappingList = new List<TileModel>();

            foreach (var pin in coordinates)
            {
                tileMappingList.Add(
                    new TileModel(
                        OSMMapHelper.WorldToTilePos(
                            pin.LongitudeDegrees,
                            pin.LatitudeDegrees,
                            this._zoom
                        )
                    )
                );
            }

            var mapStartTilePoint = new Point(
                Convert.ToInt32(tileMappingList.Min(i => i.TileCoords.X)),
                Convert.ToInt32(tileMappingList.Min(i => i.TileCoords.Y))
            );

            var mapEndTilePoint = new Point(
                Convert.ToInt32(tileMappingList.Max(i => i.TileCoords.X)),
                Convert.ToInt32(tileMappingList.Max(i => i.TileCoords.Y))
            );

            #region Extend map boundary by 1 tile on all sides
            if (mapStartTilePoint.X > 0)
            {
                mapStartTilePoint.X -= 1;
            }

            if (mapEndTilePoint.X < maxTileCountPerAxis)
            {
                mapEndTilePoint.X += 1;
            }

            if (mapStartTilePoint.Y > 0)
            {
                mapStartTilePoint.Y -= 1;
            }

            if (mapEndTilePoint.Y < maxTileCountPerAxis)
            {
                mapEndTilePoint.Y += 1;
            }
            #endregion

            var tileList = new List<TileModel>();
            for (int tX = mapStartTilePoint.X, x_offset = 0; tX <= mapEndTilePoint.X; tX++, x_offset++)
            {
                for (int tY = mapStartTilePoint.Y, y_offset = 0; tY <= mapEndTilePoint.Y; tY++, y_offset++)
                {
                    var existingTile = tileList.FirstOrDefault(i => i.TileCoords.X == tX && i.TileCoords.Y == tY);
                    if (existingTile == null)
                    {
                        existingTile = new TileModel(null)
                        {
                            RenderOffset = new Point(x_offset, y_offset),
                            TileCoords = new Point(tX, tY)
                        };
                        tileList.Add(existingTile);
                    }
                    else
                    {
                        existingTile.RenderOffset = new Point(x_offset, y_offset);
                    }
                }

            }

            await Parallel.ForEachAsync(tileList, async (tileObj, cancellationToken) =>
            {
                tileObj.TileImage = await this.GetMapTile(tileObj);
            });

            return DrawTileBitmap(tileList, coordinates, mapStartTilePoint, mapEndTilePoint, pinImage);
        }

        private async Task<Image> GetMapTile(TileModel tile)
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var address = this.TileURL
                        .Replace("{z}", this._zoom.ToString())
                        .Replace("{x}", tile.TileCoords.X.ToString())
                        .Replace("{y}", tile.TileCoords.Y.ToString());

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

        private Image DrawTileBitmap(List<TileModel> tiles, List<CoordinatesModel> coordinates, Point mapStartTile, Point mapEndTile, Image pinImage)
        {
            var bitMap = new Bitmap(
                256 * (mapEndTile.X - mapStartTile.X + 1),
                256 * (mapEndTile.Y - mapStartTile.Y + 1)
            );
            var g = Graphics.FromImage(bitMap);

            // draw map tiles
            foreach (var tile in tiles)
            {
                g.DrawImage(tile.TileImage, new Point(256 * tile.RenderOffset.X, 256 * tile.RenderOffset.Y));
            }

            var pinPositionsInImage = new List<PointF>();

            // draw pins
            if (coordinates != null)
            {
                foreach (var coord in coordinates.Where(i => i.ShowPin).OrderBy(i => i.LongitudeDegrees).ThenBy(i => i.LatitudeDegrees))
                {
                    var pinTilePos = OSMMapHelper.WorldToTilePos(coord.LongitudeDegrees, coord.LatitudeDegrees, this._zoom);

                    var tile = tiles.FirstOrDefault(i =>
                        i.TileCoords.X <= pinTilePos.X && i.TileCoords.X + 1 > pinTilePos.X &&
                        i.TileCoords.Y <= pinTilePos.Y && i.TileCoords.Y + 1 > pinTilePos.Y
                    );

                    if (tile != null)
                    {
                        var x_in_tile = 256 * (pinTilePos.X - Math.Round(pinTilePos.X, MidpointRounding.ToZero));
                        var y_in_tile = 256 * (pinTilePos.Y - Math.Round(pinTilePos.Y, MidpointRounding.ToZero));

                        var pinPosInImage = new PointF(
                            Convert.ToInt32((tile.RenderOffset.X * 256) + x_in_tile),
                            Convert.ToInt32((tile.RenderOffset.Y * 256) + y_in_tile)
                        );

                        if (pinImage != null)
                        {
                            g.DrawImage(
                                pinImage,
                                new PointF(
                                    pinPosInImage.X - (pinImage.Width / 2),
                                    pinPosInImage.Y - pinImage.Height
                                )
                            );
                        }

                        pinPositionsInImage.Add(pinPosInImage);
                    }
                }
            }

            // clip image to desired dimensions
            var medianX = pinPositionsInImage.Sum(i => i.X) / pinPositionsInImage.Count;
            var medianY = pinPositionsInImage.Sum(i => i.Y) / pinPositionsInImage.Count;

            var halfWidth = this.ImageWidth / 2;
            var halfHeight = this.ImageHeight / 2;

            var medianPosition = new PointF(
                Math.Max(0, medianX - halfWidth),
                Math.Max(0, medianY - halfHeight)
            );

            var clipRect = new RectangleF(medianPosition.X, medianPosition.Y, this.ImageWidth, this.ImageHeight);
            return bitMap.Clone(clipRect, bitMap.PixelFormat);
        }
    }
}
