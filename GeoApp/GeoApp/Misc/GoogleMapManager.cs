using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using System.Threading.Tasks;

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
                    Address = $"Lat: {point.Latitude} \nLong:{point.Longitude}",
                    Position = new Position(point.Latitude, point.Longitude)
                });
            });
        }

        public static void DrawLine(ObservableCollection<Polyline> Polylines, string name, List<Point> points)
        {
            var line = new Polyline()
            {
                StrokeColor = Color.Blue,
                IsClickable = true,
                StrokeWidth = 5f,
                Tag = name,
                ZIndex = 100
            };

            var message = "";
            var index = 0;

            points.ForEach((Point point) =>
            {
                index++;
                line.Positions.Add(new Position(point.Latitude, point.Longitude));
                message += $"Coordinate{index} : Lat {point.Latitude} , Long {point.Longitude} \n";
            });

            line.Clicked += (sender, e) => {
                Application.Current.MainPage.DisplayAlert(name, message, "Okay");
            };

            Polylines.Add(line);
        }

        public static void DrawPolygon(ObservableCollection<Polygon> Polygons, string name, List<Point> points)
        {
            var polygon = new Polygon()
            {
                StrokeColor = Color.Gray,
                FillColor = Color.FromRgba(255, 0, 0, 64),
                IsClickable = true,
                StrokeWidth = 5f,
                ZIndex = 50,
                Tag = name
            };

            var message = "";
            var index = 0;

            points.ForEach((Point point) =>
            {
                index++;
                polygon.Positions.Add(new Position(point.Latitude, point.Longitude));
                message += $"Coordinate{index} : Lat {point.Latitude} , Long {point.Longitude} \n";
            });

            polygon.Clicked += (sender, e) => {
                Application.Current.MainPage.DisplayAlert(name, message, "Okay");
            };

            Polygons.Add(polygon);
        }

    }
}
