using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;

namespace GeoApp
{
    public class GoogleMapManager
    {
        public static void DropPins(ObservableCollection<Pin> Pins, string name, List<Point> points)
        {
            points.ForEach((Point point) =>
            {
                Pins.Add(new Pin
                {
                    Label = $"{name}",
                    Position = new Position(point.Latitude, point.Longitude)
                });
            });
        }

        public static void DrawLine(ObservableCollection<Polyline> Polylines,List<Point> points)
        {
            var line = new Polyline()
            {
                StrokeColor = Color.Blue,
                IsClickable = true,
                StrokeWidth = 5f,
                Tag = "PolyLine"
            };

            points.ForEach((Point point) =>
            {
                line.Positions.Add(new Position(point.Latitude, point.Longitude));
            });

            Polylines.Add(line);
        }

        public static void DrawPolygon(ObservableCollection<Polygon> Polygons, List<Point> points)
        {
            var polygon = new Polygon()
            {
                StrokeColor = Color.Blue,
                FillColor = Color.FromRgba(255, 0, 0, 64),
                IsClickable = true,
                StrokeWidth = 5f,
                Tag = "Polygon"
            };

            points.ForEach((Point point) =>
            {
                polygon.Positions.Add(new Position(point.Latitude, point.Longitude));
            });

            Polygons.Add(polygon);
        }

        public static async void UpdateRegionToUserLocation(MapSpan region)
        {
            try
            {
                // Gets current location of device (MORE ACCURATE, but slower)
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    region = MapSpan.FromCenterAndRadius(
                        new Position( location.Latitude, location.Longitude),
                        Distance.FromKilometers(2)
                    );
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                throw fnsEx;
            }
            catch (PermissionException pEx)
            {
                throw pEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
