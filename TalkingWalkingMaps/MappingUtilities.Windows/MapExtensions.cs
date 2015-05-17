using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MappingUtilities
{
  /// <summary>
  /// Helper methods to easily set and get view area in a more or less consistent way
  /// </summary>
  public static class MapExtensions
  {
    public static LocationRect GetViewArea(this Map map)
    {
      Location p1, p2;
      map.TryPixelToLocation( new Point(0,0), out p1);
      map.TryPixelToLocation(new Point(map.ActualWidth, map.ActualHeight), out p2);
      return new LocationRect(p1, p2);
    }

    public static void SetViewArea(this Map map, Geopoint p1, Geopoint p2)
    {
      var loc1 = new Location { Latitude = Math.Max(p1.Position.Latitude, p2.Position.Latitude), Longitude = Math.Min(p1.Position.Longitude, p2.Position.Longitude) };
      var loc2 = new Location { Latitude = Math.Min(p1.Position.Latitude, p2.Position.Latitude), Longitude = Math.Max(p1.Position.Longitude, p2.Position.Longitude) };
      map.SetView( new LocationRect(loc1, loc2), TimeSpan.FromSeconds(2));
    }
  }
}
