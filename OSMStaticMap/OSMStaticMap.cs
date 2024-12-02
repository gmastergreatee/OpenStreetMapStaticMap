using OSMStaticMap.Helpers;
using OSMStaticMap.Models;
using System.Drawing;

namespace OSMStaticMap
{
    public class OSMStaticMap
    {
        public string TileURL { get; private set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ZoomLevel { get; set; }
        public byte MaxZoom { get; set; }

        public OSMStaticMap(string URL, int imageWidth, int imageHeight)
        {
            this.TileURL = URL;
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            this.ZoomLevel = 0;
        }

        public async Task<Image> PlotMapAsync(List<CoordinatesModel> coordinates, Image? pinImage = null)
        {
            this.ZoomLevel = this.CalculateZoom(coordinates, this.ImageWidth, this.ImageHeight);

            var maxTileCountPerAxis = Math.Pow(2, this.ZoomLevel);
            var tileMappingList = new List<TileModel>();

            foreach (var pin in coordinates)
            {
                tileMappingList.Add(
                    new TileModel(
                        OSMMapHelper.WorldToTilePos(
                            pin.LongitudeDegrees,
                            pin.LatitudeDegrees,
                            this.ZoomLevel
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

        public Image PlotMap(List<CoordinatesModel> coordinates, Image? pinImage = null)
        {
            return PlotMapAsync(coordinates, pinImage).GetAwaiter().GetResult();
        }

        private int CalculateZoom(List<CoordinatesModel> coordinates, int imageWidth, int imageHeight)
        {
            var tempZoom = this.ZoomLevel;
            if (this.ZoomLevel <= 0)
            {
                var minLatitude = coordinates.Min(i => i.LatitudeDegrees);
                var minLongitude = coordinates.Min(i => i.LongitudeDegrees);
                var maxLatitude = coordinates.Max(i => i.LatitudeDegrees);
                var maxLongitude = coordinates.Max(i => i.LongitudeDegrees);

                // automatically determine zoom level
                tempZoom = OSMMapHelper.CalculateZoomLevel(
                    minLatitude,
                    maxLatitude,
                    minLongitude,
                    maxLongitude,
                    imageWidth,
                    imageHeight,
                    this.MaxZoom
                );
            }

            return tempZoom;
        }

        private async Task<Image> GetMapTile(TileModel tile)
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var address = this.TileURL
                        .Replace("{z}", this.ZoomLevel.ToString())
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

        private Image DrawTileBitmap(List<TileModel> tiles, List<CoordinatesModel> coordinates, Point mapStartTile, Point mapEndTile, Image? pinImage)
        {
            if (tiles == null || tiles.Count <= 0 || coordinates == null || coordinates.Count <= 0)
            {
                return new Bitmap(this.ImageWidth, this.ImageHeight);
            }

            var bitMap = new Bitmap(
                256 * (mapEndTile.X - mapStartTile.X + 1),
                256 * (mapEndTile.Y - mapStartTile.Y + 1)
            );

            var positionsToKeepInMap = new List<PointF>();
            var pinPositionsInImage = new List<PointF>();

            foreach (var coord in coordinates)
            {
                var pinTilePos = OSMMapHelper.WorldToTilePos(coord.LongitudeDegrees, coord.LatitudeDegrees, this.ZoomLevel);

                var tile = tiles.FirstOrDefault(i =>
                    i.TileCoords.X <= pinTilePos.X && i.TileCoords.X + 1 > pinTilePos.X &&
                    i.TileCoords.Y <= pinTilePos.Y && i.TileCoords.Y + 1 > pinTilePos.Y
                );

                if (coord.ShowPin && tile != null)
                {
                    var x_in_tile = 256 * (pinTilePos.X - Math.Round(pinTilePos.X, MidpointRounding.ToZero));
                    var y_in_tile = 256 * (pinTilePos.Y - Math.Round(pinTilePos.Y, MidpointRounding.ToZero));

                    var pinPosInImage = new PointF(
                        Convert.ToInt32((tile.RenderOffset.X * 256) + x_in_tile),
                        Convert.ToInt32((tile.RenderOffset.Y * 256) + y_in_tile)
                    );

                    positionsToKeepInMap.Add(pinPosInImage);
                    if (pinImage != null)
                    {
                        pinPositionsInImage.Add(new PointF(
                            pinPosInImage.X - (pinImage.Width / 2),
                            pinPosInImage.Y - pinImage.Height
                        ));
                    }
                }
            }

            // clip image to desired dimensions
            var medianX = (positionsToKeepInMap.Max(i => i.X) + positionsToKeepInMap.Min(i => i.X)) / 2;
            var medianY = (positionsToKeepInMap.Max(i => i.Y) + positionsToKeepInMap.Min(i => i.Y)) / 2;

            var halfWidth = this.ImageWidth / 2;
            var halfHeight = this.ImageHeight / 2;

            var medianPosition = new PointF(
                Math.Max(0, medianX - halfWidth),
                Math.Max(0, medianY - halfHeight)
            );

            var clipRect = new RectangleF(
                medianPosition.X,
                medianPosition.Y,
                Math.Min(this.ImageWidth, bitMap.Width),
                Math.Min(this.ImageHeight, bitMap.Height)
            );

            var g = Graphics.FromImage(bitMap);

            // filter out unused tiles
            var usedTiles = tiles.Where(tile =>
                (
                    ((tile.RenderOffset.X * 256) <= clipRect.X && ((tile.RenderOffset.X * 256) + 256) >= clipRect.X) ||
                    ((tile.RenderOffset.X * 256) >= clipRect.X && ((tile.RenderOffset.X * 256) + 256) <= (clipRect.X + clipRect.Width)) ||
                    ((tile.RenderOffset.X * 256) <= (clipRect.X + clipRect.Width) && ((tile.RenderOffset.X * 256) + 256) >= (clipRect.X + clipRect.Width))
                ) &&
                (
                    ((tile.RenderOffset.Y * 256) <= clipRect.Y && ((tile.RenderOffset.X * 256) + 256) >= clipRect.Y) ||
                    ((tile.RenderOffset.Y * 256) >= clipRect.Y && ((tile.RenderOffset.X * 256) + 256) <= (clipRect.Y + clipRect.Height)) ||
                    ((tile.RenderOffset.Y * 256) <= (clipRect.Y + clipRect.Height) && ((tile.RenderOffset.X * 256) + 256) >= (clipRect.Y + clipRect.Height))
                )

            );

            // draw map tiles
            foreach (var tile in usedTiles)
            {
                if (tile.TileImage != null)
                {
                    g.DrawImage(tile.TileImage, new Point(256 * tile.RenderOffset.X, 256 * tile.RenderOffset.Y));
                }
            }

            // draw pins
            if (pinImage != null && pinPositionsInImage.Count > 0)
            {
                foreach (var pin in pinPositionsInImage)
                {
                    g.DrawImage(
                        pinImage,
                        pin
                    );
                }
            }

            // dont cutout map if calculated map dimensions are smaller than required
            if (bitMap.Width < clipRect.Width || bitMap.Height < clipRect.Height)
            {
                return bitMap;
            }

            return bitMap.Clone(clipRect, bitMap.PixelFormat);
        }
    }
}
