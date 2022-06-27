using Geocoding;
using Geocoding.Google;

namespace ShareInvest.Server
{
    static class Geo
    {
        internal static async Task<(string, string, double, double)> GetLocation(string address)
        {
            foreach (var lo in await geo.GeocodeAsync(address))
                return (lo.Provider, lo.FormattedAddress, lo.Coordinates.Latitude, lo.Coordinates.Longitude);

            return (string.Empty, string.Empty, double.NaN, double.NaN);
        }
        static readonly IGeocoder geo = new GoogleGeocoder
        {
            Language = "ko",
            ApiKey = Properties.Resources.google_key
        };
    }
}