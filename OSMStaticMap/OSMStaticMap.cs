using Newtonsoft.Json;
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
            this.TileURL = string.IsNullOrWhiteSpace(URL) ? "https://tile.openstreetmap.org/{z}/{x}/{y}.png" : URL;
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            this.ZoomLevel = 0;
        }

        public Image PlotMap(IEnumerable<CoordinatesModel> coordinates, Image pinImage)
        {
            this.ZoomLevel = this.CalculateZoom(
                coordinates,
                this.ImageWidth - (pinImage != null ? pinImage.Width / 2 : 0),
                this.ImageHeight - (pinImage != null ? pinImage.Height / 2 : 0)
            );

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
            var extendedBoundary = Convert.ToInt32(this.ZoomLevel / 6.0);
            if (mapStartTilePoint.X > 0)
            {
                mapStartTilePoint.X -= extendedBoundary;
            }

            if (mapEndTilePoint.X < maxTileCountPerAxis)
            {
                mapEndTilePoint.X += extendedBoundary;
            }

            if (mapStartTilePoint.Y > 0)
            {
                mapStartTilePoint.Y -= extendedBoundary;
            }

            if (mapEndTilePoint.Y < maxTileCountPerAxis)
            {
                mapEndTilePoint.Y += extendedBoundary;
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

            // initialize BitMap object and clipping mask
            var bitMap = new Bitmap(
                256 * (mapEndTilePoint.X - mapStartTilePoint.X + 1),
                256 * (mapEndTilePoint.Y - mapStartTilePoint.Y + 1)
            );
            var clipRectObject = this.GetClipRectangle(tileList, coordinates, pinImage, bitMap.Width, bitMap.Height);

            var result = Task.Run(async () =>
            {
                for (var i = 0; i < tileList.Count; i++)
                {
                    var tile = tileList[i];
                    // filter out unused tiles
                    if (this.IsTileWithinRect(tile, clipRectObject.ClippedRectangle))
                    {
                        tile.TileImage = await this.GetMapTile(tile);
                    }
                }

                return true;
            }).ConfigureAwait(false);

            var fetchTileimages = result.GetAwaiter().GetResult();

            return DrawTileBitmap(bitMap, tileList, coordinates, clipRectObject, pinImage);
        }

        private int CalculateZoom(IEnumerable<CoordinatesModel> coordinates, int imageWidth, int imageHeight)
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
                    var imageStream = await response.Content.ReadAsStreamAsync();
                    return Bitmap.FromStream(imageStream);
                }
            }
            catch
            {
                return new Bitmap(256, 256);
            }
        }

        private bool IsTileWithinRect(TileModel tile, RectangleF clipRect)
        {
            return (
                ((tile.RenderOffset.X * 256) <= clipRect.X && ((tile.RenderOffset.X * 256) + 256) >= clipRect.X) ||
                ((tile.RenderOffset.X * 256) >= clipRect.X && ((tile.RenderOffset.X * 256) + 256) <= (clipRect.X + clipRect.Width)) ||
                ((tile.RenderOffset.X * 256) <= (clipRect.X + clipRect.Width) && ((tile.RenderOffset.X * 256) + 256) >= (clipRect.X + clipRect.Width))
            ) &&
            (
                ((tile.RenderOffset.Y * 256) <= clipRect.Y && ((tile.RenderOffset.Y * 256) + 256) >= clipRect.Y) ||
                ((tile.RenderOffset.Y * 256) >= clipRect.Y && ((tile.RenderOffset.Y * 256) + 256) <= (clipRect.Y + clipRect.Height)) ||
                ((tile.RenderOffset.Y * 256) <= (clipRect.Y + clipRect.Height) && ((tile.RenderOffset.Y * 256) + 256) >= (clipRect.Y + clipRect.Height))
            );
        }

        private Image DrawTileBitmap(Bitmap bitMap, IEnumerable<TileModel> tiles, IEnumerable<CoordinatesModel> coordinates, ClippedRectangleModel clipRectObject, Image pinImage)
        {
            if (tiles == null || tiles.Count() <= 0 || coordinates == null || coordinates.Count() <= 0)
            {
                return new Bitmap(this.ImageWidth, this.ImageHeight);
            }

            using (var g = Graphics.FromImage(bitMap))
            {
                // draw map tiles
                foreach (var tile in tiles)
                {
                    if (tile.TileImage != null)
                    {
                        g.DrawImage(tile.TileImage, new Point(256 * tile.RenderOffset.X, 256 * tile.RenderOffset.Y));
                    }
                }

                // draw pins
                if (pinImage != null && clipRectObject.PinPositions.Count > 0)
                {
                    foreach (var pin in clipRectObject.PinPositions)
                    {
                        g.DrawImage(
                            pinImage,
                            pin
                        );
                    }
                }
            }

            var finalRectangleClip = new RectangleF(
                clipRectObject.ClippedRectangle.X,
                clipRectObject.ClippedRectangle.Y,
                clipRectObject.ClippedRectangle.Width,
                clipRectObject.ClippedRectangle.Height
            );

            if (bitMap.Width < clipRectObject.ClippedRectangle.X + clipRectObject.ClippedRectangle.Width)
            {
                finalRectangleClip.Width = Math.Min(bitMap.Width, finalRectangleClip.Width);
                finalRectangleClip.X = bitMap.Width - finalRectangleClip.Width;
            }

            if (bitMap.Height < clipRectObject.ClippedRectangle.Y + clipRectObject.ClippedRectangle.Height)
            {
                finalRectangleClip.Height = Math.Min(bitMap.Height, finalRectangleClip.Height);
                finalRectangleClip.Y = bitMap.Height - finalRectangleClip.Height;
            }

            return bitMap.Clone(finalRectangleClip, bitMap.PixelFormat);
        }

        private ClippedRectangleModel GetClipRectangle(IEnumerable<TileModel> tiles, IEnumerable<CoordinatesModel> coordinates, Image pinImage, int imageWidth, int imageHeight)
        {
            var positionsToKeepInMap = new List<PointF>();
            var pinPositionsInMap = new List<PointF>();

            foreach (var coord in coordinates)
            {
                var pinTilePos = OSMMapHelper.WorldToTilePos(coord.LongitudeDegrees, coord.LatitudeDegrees, this.ZoomLevel);

                var tile = tiles.FirstOrDefault(i =>
                    i.TileCoords.X <= pinTilePos.X && i.TileCoords.X + 1 > pinTilePos.X &&
                    i.TileCoords.Y <= pinTilePos.Y && i.TileCoords.Y + 1 > pinTilePos.Y
                );

                if (coord.ShowPin && tile != null)
                {
                    var x_in_tile = 256 * (pinTilePos.X - Math.Floor(pinTilePos.X));
                    var y_in_tile = 256 * (pinTilePos.Y - Math.Floor(pinTilePos.Y));

                    var pinPosInImage = new PointF(
                        Convert.ToInt32((tile.RenderOffset.X * 256) + x_in_tile),
                        Convert.ToInt32((tile.RenderOffset.Y * 256) + y_in_tile)
                    );

                    positionsToKeepInMap.Add(pinPosInImage);
                    if (pinImage != null)
                    {
                        pinPositionsInMap.Add(new PointF(
                            pinPosInImage.X - (pinImage.Width / 2),
                            pinPosInImage.Y - pinImage.Height
                        ));
                    }
                }
            }

            // clip image to desired dimensions
            float medianX = 0;
            float medianY = 0;

            medianX = (positionsToKeepInMap.Max(i => i.X) + positionsToKeepInMap.Min(i => i.X)) / 2;
            medianY = ((positionsToKeepInMap.Max(i => i.Y) + positionsToKeepInMap.Min(i => i.Y)) / 2);

            // offset the map center according to pin-image's width and height
            if (pinImage != null)
            {
                medianY -= pinImage.Height / 2;
            }

            var halfWidth = this.ImageWidth / 2;
            var halfHeight = this.ImageHeight / 2;

            var medianPosition = new PointF(
                Math.Max(0, medianX - halfWidth),
                Math.Max(0, medianY - halfHeight)
            );


            return new ClippedRectangleModel()
            {
                ClippedRectangle = new RectangleF(
                    medianPosition.X,
                    medianPosition.Y,
                    Math.Min(this.ImageWidth, imageWidth),
                    Math.Min(this.ImageHeight, imageHeight)
                ),
                PinPositions = pinPositionsInMap,
            };
        }

        #region Static methods
        public static IEnumerable<CoordinatesModel> GetCoordinates(IEnumerable<string> addresses)
        {
            var coordinates = new List<PointF>();
            var lastQueryTimestamp = DateTime.Now;
            var result = Task.Run(async () =>
            {
                foreach (var address in addresses)
                {
                    try
                    {
                        using (var hc = new HttpClient())
                        {
                            hc.DefaultRequestHeaders.UserAgent.ParseAdd("Chrome/131.0.0.0");
                            var url = $"https://nominatim.openstreetmap.org/search?format=json&limit=1&q={Uri.EscapeDataString(address)}";

                            // nominatim.openstreetmap.org/search
                            // enforces at-most 1 request per second
                            // else it blocks the application based on the UserAgent
                            // https://operations.osmfoundation.org/policies/nominatim/
                            // -- change the user-agent if you are blocked
                            var durationFromLastQuery = DateTime.Now - lastQueryTimestamp;
                            while (durationFromLastQuery.TotalSeconds < 1)
                            {
                                Thread.Sleep(100);
                                durationFromLastQuery = DateTime.Now - lastQueryTimestamp;
                            }

                            var response = await hc.GetAsync(url);
                            var content = await response.Content.ReadAsStringAsync();
                            lastQueryTimestamp = DateTime.Now;

                            var data = JsonConvert.DeserializeObject<List<dynamic>>(content);
                            if (data != null && data.Count > 0)
                            {
                                coordinates.Add(new PointF(Convert.ToSingle(data[0].lat), Convert.ToSingle(data[0].lon)));
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
                return true;
            });
            var fetchCoordinates = result.GetAwaiter().GetResult();
            return coordinates.Select(i => CoordinatesModel.FromPointF(i));
        }

        /// <summary>
        /// Gets the coordinates and returns model collection of those which were modified
        /// </summary>
        /// <typeparam name="T">ICoordinates object</typeparam>
        /// <param name="modelsToCheckCoordinatesFor">The model collection to check for</param>
        /// <returns>Collection containing models that were modified</returns>
        public static IEnumerable<T> GetCoordinates<T>(IEnumerable<T> modelsToCheckCoordinatesFor)
            where T : ICoordinates
        {
            var modifiedModels = new List<T>();
            foreach (var model in modelsToCheckCoordinatesFor)
            {
                if (model.Latitude == null || model.Longitude == null)
                {
                    var _address = model.GetAddress();
                    var coordinates = OSMStaticMap.GetCoordinates(new List<string> { _address });
                    if (coordinates.Count() > 0)
                    {
                        model.Latitude = coordinates.FirstOrDefault().LatitudeDegrees.ToString();
                        model.Longitude = coordinates.FirstOrDefault().LongitudeDegrees.ToString();
                    }
                    else
                    {
                        model.Latitude = "NOT FOUND";
                        model.Longitude = "NOT FOUND";
                    }
                    modifiedModels.Add(model);
                }
            }
            return modifiedModels;
        }

        public static string GetMapBase64StringFromAddresses(IEnumerable<string> addresses, string markerIconPath, int imageWidth = 1050, int imageHeight = 520)
        {
            var coordinates = OSMStaticMap.GetCoordinates(addresses);
            return GetMapBase64StringFromLatLong(coordinates, markerIconPath, imageWidth, imageHeight);
        }

        public static string GetMapBase64StringFromLatLong(IEnumerable<CoordinatesModel> coordinates, string markerIconPath, int imageWidth = 1050, int imageHeight = 520)
        {
            if (coordinates.Count() > 0 && !string.IsNullOrWhiteSpace(markerIconPath))
            {
                var pinImage = Bitmap.FromFile(markerIconPath);
                var osmMapObject = new OSMStaticMap(null, imageWidth, imageHeight)
                {
                    MaxZoom = 16
                };

                var fullImage = osmMapObject.PlotMap(
                    coordinates,
                    pinImage
                );

                using (var ms = new MemoryStream())
                {
                    fullImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    var imageBytes = ms.ToArray();

                    if (imageBytes.Length > 0)
                    {
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
