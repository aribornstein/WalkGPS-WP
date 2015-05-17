using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MappingUtilities
{
  /// <summary>
  /// Helper methods to easily convert Geopoint Windows 8 Location and back
  /// </summary>
  public static class PointHelpers
  {
    public static LocationCollection ToLocationCollection(this IList<Geopoint> pointList)
    {
      var locations = new LocationCollection();
      foreach (var p in pointList)
      {
        locations.Add(p.ToLocation());
      }
      return locations;
    }

    public static IList<Geopoint> ToGeopointList(this LocationCollection pointList)
    {
      var locations = new List<Geopoint>();
      foreach (var p in pointList)
      {
        locations.Add(p.ToGeopoint());
      }
      return locations;
    }

    public static Geopoint ToGeopoint( this Location location )
    {
      return new Geopoint( new BasicGeoposition{ Latitude = location.Latitude, Longitude = location.Longitude});
    }

    public static Location ToLocation(this Geopoint location)
    {
      return new Location(location.Position.Latitude, location.Position.Longitude);
    }
  }
}
