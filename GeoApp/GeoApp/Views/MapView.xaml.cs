using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapView : ContentPage
    {
        public MapView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.47004901089882, 153.021072), Distance.FromMiles(1.0)));
            map.MapClicked += OnMapClicked;
        }

        private void CleanFeaturesOnMap()
        {
            map.MapElements.Clear();
            map.Pins.Clear();
        }

        public void DrawAllGeoDataOnTheMap()
        {
            // Using CurrentFeature to draw the geodata on the map
            App.FeatureStore.CurrentFeatures.ForEach((Feature feature) =>
            {
                var points = feature.Properties.Xamarincoordinates;

                // One day before the feature, so it works for showing all feature
                DateTime beforeDate = DateTime.Parse(feature.Properties.Date).AddDays(-1);

                //if (Date_filter.Equals("Today"))
                //    beforeDate = DateTime.Today.AddDays(-1);
                //else if (Date_filter.Equals("Last 7 days"))
                //    beforeDate = DateTime.Now.AddDays(-7);
                //else if (Date_filter.Equals("Last month"))
                //    beforeDate = DateTime.Now.AddDays(-30);

                // feature is earily than before date
                //if (DateTime.Compare(beforeDate, DateTime.Parse(feature.properties.date)) < 0)
                //{
                if (feature.Geometry.Type.Equals("Point"))
                {
                    Pin pin = new Pin
                    {
                        Label = feature.Properties.Name,
                        Address = string.Format("{0}, {1}, {2}", points[0].Latitude, points[0].Longitude, points[0].Altitude),
                        Type = PinType.Place,
                        Position = new Position(points[0].Latitude, points[0].Longitude)
                    };
                    map.Pins.Add(pin);
                }
                else if (feature.Geometry.Type.Equals("Line"))
                {
                    Polyline polyline = new Polyline
                    {
                        StrokeColor = Color.OrangeRed,
                        StrokeWidth = 5,
                    };
                    points.ForEach((Point point) =>
                    {
                        polyline.Geopath.Add(new Position(point.Latitude, point.Longitude));
                    });
                    map.MapElements.Add(polyline);
                }
                else if (feature.Geometry.Type.Equals("Polygon"))
                {
                    Polygon polygon = new Polygon
                    {
                        StrokeWidth = 4,
                        StrokeColor = Color.OrangeRed,
                        FillColor = Color.FromHex("#85cb5748"),
                    };
                    points.ForEach((Point point) =>
                    {
                        polygon.Geopath.Add(new Position(point.Latitude, point.Longitude));
                    });

                    map.MapElements.Add(polygon);
                }
                //}
            });
        }

        void OnMapClicked(object sender, MapClickedEventArgs e)
        {

            App.FeatureStore.CurrentFeatures.ForEach((Feature feature) =>
            {
                bool ItemHit = false;
                Point[] points = feature.Properties.Xamarincoordinates.ToArray();
                if (feature.Geometry.Type.Equals("Polygon"))
                {
                    ItemHit |= IsPointInPolygon(new Point(e.Position.Latitude, e.Position.Longitude, 0), points);
                }
                else if (feature.Geometry.Type.Equals("Line"))
                {
                    ItemHit |= IsPointOnLine(new Point(e.Position.Latitude, e.Position.Longitude, 0), points);
                }

                if (ItemHit)
                {
                    string pointString = "";
                    for (int i = 0; i < points.Length; i++)
                    {
                        pointString += string.Format("{0}, {1}, {2} \n", points[i].Latitude, points[i].Longitude, points[i].Altitude);
                    }
                    HomePage.Instance.DisplayAlert(feature.Properties.Name, pointString, "Dismiss");
                }
            });
        }

        public bool IsPointInPolygon(Point p, Point[] polygon)
        {
            double minX = polygon[0].Longitude;
            double maxX = polygon[0].Longitude;
            double minY = polygon[0].Latitude;
            double maxY = polygon[0].Latitude;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.Longitude, minX);
                maxX = Math.Max(q.Longitude, maxX);
                minY = Math.Min(q.Latitude, minY);
                maxY = Math.Max(q.Latitude, maxY);
            }

            if (p.Longitude < minX || p.Longitude > maxX || p.Latitude < minY || p.Latitude > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Latitude > p.Latitude) != (polygon[j].Latitude > p.Latitude) &&
                     p.Longitude < (polygon[j].Longitude - polygon[i].Longitude) * (p.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        //currently only works on line vertices
        public bool IsPointOnLine(Point p, Point[] polyline)
        {
            for (int i = 0; i < polyline.Length; i++)
            {
                Point q = polyline[i];
                if (Math.Abs(p.Latitude - q.Latitude) <= .0003 && Math.Abs(p.Longitude - q.Longitude) <= .0003)
                {
                    return true;
                }
            }
            return false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CleanFeaturesOnMap();
            DrawAllGeoDataOnTheMap();

            if (CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location).Result == PermissionStatus.Granted)
            {
                map.IsShowingUser = true;
            }
            else
            {
                map.IsShowingUser = false;
                CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            }
        }
    }
}