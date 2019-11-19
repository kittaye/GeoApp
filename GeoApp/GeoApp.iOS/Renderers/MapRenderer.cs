using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using CoreLocation;
using Foundation;
using GeoApp;
using GeoApp.iOS;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace GeoApp.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        MKCircleRenderer circleRenderer;
        UIView _view = null;
        UIView customPinView;



        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveOverlays(nativeMap.Overlays);
                    nativeMap.OverlayRenderer = null;
                    circleRenderer = null;
                }
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                var position = formsMap.Position;
                CLLocationCoordinate2D[] coordinates = {
                    new CLLocationCoordinate2D(latitude: 45.522585, longitude: -122.685699),
                    new CLLocationCoordinate2D(latitude: 45.534611, longitude: -122.708873),
                    new CLLocationCoordinate2D(latitude: 45.530883, longitude: -122.678833),
                    new CLLocationCoordinate2D(latitude: 45.547115, longitude: -122.667503),
                    new CLLocationCoordinate2D(latitude: 45.530643, longitude: -122.660121),
                    new CLLocationCoordinate2D(latitude: 45.533529, longitude: -122.636260),
                    new CLLocationCoordinate2D(latitude: 45.521743, longitude: -122.659091),
                    new CLLocationCoordinate2D(latitude: 45.510677, longitude: -122.648792),
                    new CLLocationCoordinate2D(latitude: 45.515008, longitude: -122.664070),
                    new CLLocationCoordinate2D(latitude: 45.502496, longitude: -122.669048),
                    new CLLocationCoordinate2D(latitude: 45.515369, longitude: -122.678489),
                    new CLLocationCoordinate2D(latitude: 45.506346, longitude: -122.702007),
                    new CLLocationCoordinate2D(latitude: 45.522585, longitude: -122.685699)
                };


                MKPolygon hotelOverlay = MKPolygon.FromCoordinates(coordinates);
                nativeMap.AddOverlay(hotelOverlay);
                var polygon = MKPolygon.FromCoordinates(coordinates);
                var renderer = new MKPolygonRenderer(polygon) { FillColor = UIColor.Red, Alpha = 0.5f };
                nativeMap.OverlayRenderer = (view, overlay) => renderer;

                //nativeMap.OverlayRenderer = GetOverlayRenderer;

                //var circleOverlay = MKCircle.Circle(new CoreLocation.CLLocationCoordinate2D(position.Latitude, position.Longitude), 1000);
                //nativeMap.AddOverlay(circleOverlay);
                //nativeMap.AddGestureRecognizer(new UITapGestureRecognizer(TapHandle));

            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            //if (circleRenderer == null && !Equals(overlayWrapper, null))
            //{
                var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
                circleRenderer = new MKCircleRenderer(overlay as MKCircle)
                {
                    FillColor = UIColor.Red,
                    Alpha = 0.4f
                };
            //}
            return circleRenderer;
        }

        void TapHandle(UITapGestureRecognizer tap)
        {
            MKMapView mapView = tap.View as MKMapView;

            CGPoint tapPoint = tap.LocationInView(mapView);
            CLLocationCoordinate2D tapCoordinate = mapView.ConvertPoint(tapPoint, tap.View);
            MKMapPoint point = MKMapPoint.FromCoordinate(tapCoordinate);

            foreach (IMKOverlay overlay in mapView.Overlays)
            {
                MKCircleRenderer render = GetOverlayRenderer(mapView, overlay) as MKCircleRenderer;
                CGPoint datPoint = render.PointForMapPoint(point);
                render.InvalidatePath();

                if (render.Path.ContainsPoint(datPoint, false))
                {
                    Debug.WriteLine("hiiiii");

                }
            }
        }



    }
}
