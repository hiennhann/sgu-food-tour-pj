using Microsoft.Maui.Devices.Sensors;

namespace VinhKhanhTour.Utilities
{
    public static class LocationHelper
    {
        /// <summary>
        /// Tính khoảng cách giữa 2 tọa độ GPS ra đơn vị Mét (m)
        /// </summary>
        public static double CalculateDistanceInMeters(double userLat, double userLon, double targetLat, double targetLon)
        {
            var userLocation = new Location(userLat, userLon);
            var targetLocation = new Location(targetLat, targetLon);

            // MAUI trả về khoảng cách theo Kilometers, ta nhân 1000 để ra Mét
            double distanceInKm = Location.CalculateDistance(userLocation, targetLocation, DistanceUnits.Kilometers);

            return distanceInKm * 1000;
        }
    }
}