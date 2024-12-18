
using OSMStaticMap.Models;

namespace OpenStreetMapStaticMaps
{
    public partial class MainWindow : Form
    {
        private List<CoordinatesModel> coordinateList = new();
        private OSMStaticMap.OSMStaticMap mapper;

        private int ImageWidth = 512;
        private int ImageHeight = 512;

        public MainWindow()
        {
            InitializeComponent();
            mapper = new OSMStaticMap.OSMStaticMap(
                "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
                this.ImageWidth,
                this.ImageHeight
            );
        }

        private void btnDrawIt_Click(object sender, EventArgs e)
        {
            btnDrawIt.Enabled = false;
            CalculatePins();

            if (coordinateList.Count > 0)
            {
                var pinImage = Bitmap.FromFile("./spotlight_pin_v4_dot-2-medium.png");
                mapper.ImageWidth = this.ImageWidth;
                mapper.ImageHeight = this.ImageHeight;
                mapper.ZoomLevel = 0;
                mapper.MaxZoom = Convert.ToByte(numericMaxZoom.Value);

                coordinateList.ForEach(coord =>
                {
                    coord.ShowPin = true;
                });

                Task.Run(() =>
                {
                    var img = mapper.PlotMap(coordinateList, pinImage);
                    this.Invoke(() =>
                    {
                        picMapImage.Image = img;
                        txtZoom.Text = mapper.ZoomLevel.ToString();
                        btnDrawIt.Enabled = true;
                    });
                });
            }
            else
            {
                MessageBox.Show("No pins to mark!");
                btnDrawIt.Enabled = true;
            }
        }

        private void CalculatePins()
        {
            coordinateList.Clear();

            try
            {
                if (txtLon1.Text.Length > 0 && txtLat1.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat1.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon1.Text),
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon2.Text.Length > 0 && txtLat2.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat2.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon2.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon3.Text.Length > 0 && txtLat3.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat3.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon3.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon4.Text.Length > 0 && txtLat4.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat4.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon4.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon5.Text.Length > 0 && txtLat5.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat5.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon5.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon6.Text.Length > 0 && txtLat6.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat6.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon6.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon7.Text.Length > 0 && txtLat7.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat7.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon7.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon8.Text.Length > 0 && txtLat8.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat8.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon8.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon9.Text.Length > 0 && txtLat9.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat9.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon9.Text)
                    });
                }
            }
            catch { }

            try
            {
                if (txtLon10.Text.Length > 0 && txtLat10.Text.Length > 0)
                {
                    coordinateList.Add(new CoordinatesModel()
                    {
                        LatitudeDegrees = Convert.ToSingle(txtLat10.Text),
                        LongitudeDegrees = Convert.ToSingle(txtLon10.Text)
                    });
                }
            }
            catch { }

            try
            {
                this.ImageWidth = Convert.ToInt32(txtMapWidth.Text);
            }
            catch
            {
                this.ImageWidth = 512;
            }

            try
            {
                this.ImageHeight = Convert.ToInt32(txtMapHeight.Text);
            }
            catch
            {
                this.ImageHeight = 512;
            }
        }
    }
}
