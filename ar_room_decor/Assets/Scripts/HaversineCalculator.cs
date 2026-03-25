public static class HaversineCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public static double CalculateDistance(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                   System.Math.Cos(ToRadians(lat1)) *
                   System.Math.Cos(ToRadians(lat2)) *
                   System.Math.Sin(dLon / 2) *
                   System.Math.Sin(dLon / 2);

        double c = 2 * System.Math.Atan2(
                       System.Math.Sqrt(a),
                       System.Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * System.Math.PI / 180.0;
    }
}