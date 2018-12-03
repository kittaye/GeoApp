using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    }
}
