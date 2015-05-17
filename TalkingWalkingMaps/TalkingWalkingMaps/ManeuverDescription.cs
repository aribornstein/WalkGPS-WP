using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace TalkingWalkingMaps
{
  public class ManeuverDescription
  {
    public string Id { get; set; }

    public Geopoint  Location { get; set; }

    public string Description { get; set; }
  }
}
