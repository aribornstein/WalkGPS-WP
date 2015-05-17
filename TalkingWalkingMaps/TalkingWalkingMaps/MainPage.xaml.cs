using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Devices.Sensors;
using TalkingWalkingMaps.Common;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TalkingWalkingMaps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool routing = false;
        private Geolocator geolocator;
        private Compass myCompass = Compass.GetDefault(); 
        const int carZIndewxz = 4;
        MapRoute route;
        private List<ManeuverDescription> maneuverList = new List<ManeuverDescription>();
        public  MainPage()
        {
            this.InitializeComponent();
            routing = false;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.drawStartMap();
            this.beginTracking();
        }
        //tracking 
        public  void beginTracking()
        {
            geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 1
            };
            geolocator.PositionChanged += GeolocatorPositionChanged;
            GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GeolocatorPositionChanged(args.Position); // calls back to not shared portion
            });
        }
        private async void GeolocatorPositionChanged(Geoposition point)
        {
            // ... and another coordinate conversion
            var pos = new Geopoint(new BasicGeoposition { Latitude = point.Coordinate.Point.Position.Latitude, Longitude = point.Coordinate.Point.Position.Longitude });

            DrawCarIcon(pos);

           
            // Gives obsolete warning:
            // var pos = new Geopoint(new BasicGeoposition { Latitude = point.Coordinate.Latitude, Longitude = point.Coordinate.Longitude });
            
            await myMap.TrySetViewAsync(pos, myMap.ZoomLevel, myMap.Heading, myMap.Pitch, MapAnimationKind.Linear);
        }
        //drawing methods
        public async void drawStartMap()
        {
            //configure saved settings
            if (!(bool)App.localSettings.Values["Voice"])VoiceToggle.Label = "Enable Voice";
            
            //draw 
            Geoposition curPos = await getPosition();
            myMap.Center = curPos.Coordinate.Point;
            myMap.ZoomLevel = 18;
        }


        private void DrawRoute(MapRoute route)
        {
            //Draw a semi transparent fat green line
            var color = Colors.Green;
            color.A = 128;
            myMap.MapElements.Clear();
            var line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };

            // Route has legs, legs have maneuvers
            foreach (var leg in route.Legs)
            {
                DrawLeg(leg);
            }

            // Route has a Path containing all points to draw the route
            line.Path = new Geopath(route.Path.Positions);

            myMap.MapElements.Add(line);
        }

        private void DrawLeg(MapRouteLeg leg)
        {
            foreach (var maneuver in leg.Maneuvers)
            {
                DrawManeuver(maneuver);
            }
            //add geofences
            foreach (var maneuver in maneuverList)
            {
                AddFence(maneuver.Id.ToString(), maneuver.Location);
            }
           
        }

        private void DrawManeuver(MapRouteManeuver maneuver)
        {
            //Retain this for later use
            maneuverList.Add(new ManeuverDescription { Id = maneuverList.Count().ToString(), Location = maneuver.StartingPoint, Description = maneuver.InstructionText });
            var icon = new MapIcon
            {
                NormalizedAnchorPoint = new Point(0.5, 0.5),
                Location = maneuver.StartingPoint,
                ZIndex = 3
            };
            icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/traffic.png"));
            myMap.MapElements.Add(icon);
        }

        private void DrawCarIcon(Geopoint pos)
        {
            var carIcon = myMap.MapElements.OfType<MapIcon>().FirstOrDefault(p => p.ZIndex == carZIndewxz);
            if (carIcon == null)
            {
                carIcon = new MapIcon
                {
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    ZIndex = carZIndewxz
                };
                carIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/car.png"));
                
                myMap.MapElements.Add(carIcon);
                
            }
            carIcon.Location = pos;
        }
        //routing methods
     

        public async Task<MapRoute> routeTo(string endAddr)
        {
            Geoposition curPos = await getPosition();
            MapLocationFinderResult endAddrResult = await MapLocationFinder.FindLocationsAsync(endAddr, curPos.Coordinate.Point);
            var end = endAddrResult.Locations.First();
            MapRouteFinderResult result = await MapRouteFinder.GetWalkingRouteAsync(curPos.Coordinate.Point, end.Point);
            return result.Route;
        }
        public async Task<MapRoute> routeFrom(string startAddr)
        {
            Geoposition curPos = await getPosition();

            MapLocationFinderResult startAddrResult = await MapLocationFinder.FindLocationsAsync(startAddr, curPos.Coordinate.Point);
            var start = startAddrResult.Locations.First();
            
            MapRouteFinderResult result = await MapRouteFinder.GetWalkingRouteAsync(start.Point, curPos.Coordinate.Point);

            //set the view to start
            DrawCarIcon(start.Point);

            return result.Route;
        }
        public async Task<MapRoute> routeBetween(string startAddr, string endAddr)
        {
           Geoposition curPos = await getPosition();

           MapLocationFinderResult startAddrResult = await MapLocationFinder.FindLocationsAsync(startAddr, curPos.Coordinate.Point);
           var start = startAddrResult.Locations.First();

           MapLocationFinderResult endAddrResult = await MapLocationFinder.FindLocationsAsync(endAddr, curPos.Coordinate.Point);
           var end = endAddrResult.Locations.First();

           MapRouteFinderResult result = await MapRouteFinder.GetWalkingRouteAsync(start.Point, end.Point);

           //set the view to start
           DrawCarIcon(start.Point);
 
           return result.Route;
        }
    
        public async Task<Geoposition> getPosition()
        {
            Geolocator myLoc = new Geolocator();
            Geoposition geoPos = await  myLoc.GetGeopositionAsync(maximumAge:TimeSpan.FromSeconds(20),timeout:TimeSpan.FromSeconds(10));
            return geoPos;
        }

        public void LocationHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                //Okay Button.
                case "Allow":
                    App.localSettings.Values["LocationEnabled"] = true;
                    break;
                //Quit Button.
                case "Quit":
                    Application.Current.Exit();
                    break;
                //end.
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        /// 
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Check LocationSettings  
            if (!(bool)App.localSettings.Values["LocationEnabled"] )
            { 
                // Prompt user
                MessageDialog msg = new MessageDialog("Location services are required to use Walk GPS. Location Services can be toggled later from the \"...\" menu. ");
                msg.Commands.Add(new UICommand("Allow", new UICommandInvokedHandler(LocationHandlers)));
                msg.Commands.Add(new UICommand("Quit", new UICommandInvokedHandler(LocationHandlers)));
                await msg.ShowAsync();  
            }

           MapRoute route = null;

           Tuple<string, List<string>> data = e.Parameter as Tuple<string, List<string>>;

           if (data == null) return;
           try
            {
                //route from
                if (data.Item1 == "RouteFrom") route = await this.routeFrom(data.Item2[0]);
                //route between
                else if (data.Item1 == "RouteBetween") route = await this.routeBetween(data.Item2[0], data.Item2[1]);
                ManeuverDisplay.Clear();
                maneuverList.Clear();
                GeofenceMonitor.Current.Geofences.Clear();
                DrawRoute(route);
                GeofenceMonitor.Current.Geofences.Clear();
            }
            catch (Exception)
            {
                Windows.UI.Popups.MessageDialog message = new Windows.UI.Popups.MessageDialog("No Route Found! Please Refine Your Search.");
                 message.ShowAsync();
            }
           
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (myMap.ZoomLevel > 0) myMap.ZoomLevel = myMap.ZoomLevel - 1;
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            if (myMap.ZoomLevel < 20) myMap.ZoomLevel = myMap.ZoomLevel + 1;
        }


        private async void  Search(object sender, RoutedEventArgs e)
        {
            if (routing) return;
            routing = true;
            bool errMsg = false;

            try
            {
                route = await routeTo(SearchBox.Text);
                ManeuverDisplay.Clear();
                maneuverList.Clear();
                GeofenceMonitor.Current.Geofences.Clear();
                DrawRoute(route);
            }

            catch (Exception ) 
            {
                errMsg = true;                     
            }
            if (errMsg)
            {
                Windows.UI.Popups.MessageDialog message = new Windows.UI.Popups.MessageDialog("No Route Found! Please Refine Your Search.");
                await message.ShowAsync();
            }
            routing = false;          
        }

        /// <summary>
        /// Creation 
        /// </summary>
        /// 
        public void AddFence(string key, Geopoint position)
        {
     
            // Replace if it already exists for this maneuver key
            var oldFence = GeofenceMonitor.Current.Geofences.Where(p => p.Id == key).FirstOrDefault();
            if (oldFence != null)
            {
                GeofenceMonitor.Current.Geofences.Remove(oldFence);
            }

            Geocircle geocircle = new Geocircle(position.Position, 25);

            bool singleUse = false;

            MonitoredGeofenceStates mask = 0;

            mask |= MonitoredGeofenceStates.Entered;
            mask |= MonitoredGeofenceStates.Exited;

            var geofence = new Geofence(key, geocircle, mask, singleUse, TimeSpan.FromSeconds(1));
            GeofenceMonitor.Current.Geofences.Add(geofence);
        }

        /// <summary>
        /// Tracks actual geofencing events
        /// </summary>
        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            if (sender.Geofences.Any())
            {
                var reports = sender.ReadReports();
                foreach (var report in reports)
                {
                    switch (report.NewState)
                    {
                        case GeofenceState.Entered:
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    ManeuverDisplay.DisplayManeuver(maneuverList.Where(p => p.Id == report.Geofence.Id).First());
                                                                                                          
                                });
                                break;
                            }
                        case GeofenceState.Exited:
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    ManeuverDisplay.HideManeuver(report.Geofence.Id);
                                    //remove geofence here
                                });
                                break;
                            }
                    }
                }
            }
        }

        private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!(e.Key == Windows.System.VirtualKey.Enter)) return;
            if (routing) return;
            routing = true;
            bool errMsg = false;
            MapRoute route;
            try
            {
                route = await routeTo(SearchBox.Text);
                ManeuverDisplay.Clear();
                maneuverList.Clear();
                GeofenceMonitor.Current.Geofences.Clear();
                DrawRoute(route);
            }

            catch (Exception)
            {
                errMsg = true;
            }
            if (errMsg)
            {
                Windows.UI.Popups.MessageDialog message = new Windows.UI.Popups.MessageDialog("No Route Found! Please Refine Your Search.");
                await message.ShowAsync();
            }
            routing = false;
        }

        private async void Refresh(object sender, RoutedEventArgs e)
        {
            var heading = myCompass.GetCurrentReading().HeadingMagneticNorth;
            // ... and another coordinate conversion
            var pos = await getPosition();
            DrawCarIcon(pos.Coordinate.Point);
            await myMap.TrySetViewAsync(pos.Coordinate.Point, myMap.ZoomLevel, heading, myMap.Pitch, MapAnimationKind.Linear);
        }

        private void ListDirections(object sender, RoutedEventArgs e)
        {
            DirectionsInfo myDI = new DirectionsInfo(); 
            List<string> myDirections = new List<string>();
            foreach (ManeuverDescription md in maneuverList) myDirections.Add(md.Description);
            myDI.DirectionsList= myDirections;
            myDI.TimeEst = route.EstimatedDuration.ToString();
            this.Frame.Navigate(typeof(DirectionsList), myDI);
        }
        private void RouteToHere(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RouteFrom));
        }
        private void RouteBetween(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RouteBetween));
        }

        private void ToggleVoice(object sender, RoutedEventArgs e)
        {

            if ((bool)App.localSettings.Values["Voice"])
            {
                App.localSettings.Values["Voice"] = false;
                VoiceToggle.Label = "Enable Voice";
            }
            else
            {
                App.localSettings.Values["Voice"] = true;
                VoiceToggle.Label = "Mute Voice";
            }
        }

        private async void LocationToggle_Click(object sender, RoutedEventArgs e)
        {
            // Disable Location Services
            App.localSettings.Values["LocationEnabled"] = false;
            //Prompt User
            MessageDialog msg = new MessageDialog("Location services are required to use Walk GPS. Location Services can be toggled later from the \"...\" menu. ");
                msg.Commands.Add(new UICommand("Allow", new UICommandInvokedHandler(LocationHandlers)));
                msg.Commands.Add(new UICommand("Quit", new UICommandInvokedHandler(LocationHandlers)));
                await msg.ShowAsync();  
        }
        

        
        
    }
}
