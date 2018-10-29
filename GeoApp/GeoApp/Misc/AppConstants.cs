using System;

namespace GeoApp {
    public static class AppConstants {
        public static readonly int NEW_ENTRY_ID = -1;
        public static readonly int GPS_DIGIT_PRECISION = 9;

        public static void RoundGPSPosition(Point point) {
            point.Latitude = Math.Round(point.Latitude, GPS_DIGIT_PRECISION);
            point.Longitude = Math.Round(point.Longitude, GPS_DIGIT_PRECISION);
            point.Altitude = Math.Round(point.Altitude, GPS_DIGIT_PRECISION);
        }
    }
}
