using System;
using System.Collections.ObjectModel;
using System.Linq;
using TalkingWalkingMaps.Common;
using TalkingWalkingMaps.Common.TileContent;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace TalkingWalkingMaps
{
  public sealed partial class ManeuverDisplayer : UserControl
  {
    private ObservableCollection<ManeuverDescription> maneuvers;

    public ManeuverDisplayer()
    {
      maneuvers = new ObservableCollection<ManeuverDescription>();
      this.InitializeComponent();
      this.Loaded += ManeuverDisplayer_Loaded;
    }

    void ManeuverDisplayer_Loaded(object sender, RoutedEventArgs e)
    {
      this.DataContext = maneuvers;
      UpdateVisibility();
    }

    public async void DisplayManeuver(ManeuverDescription m)
    {
      //show manuver
      maneuvers.Add(m);
      UpdateVisibility();
      //update live tile
      this.UpdateTile(m.Description);
      //speak manuver
      if ((bool)App.localSettings.Values["Voice"])
      {
          try
          {
              //pause current song
              var  currentSong = Windows.Media.Playback.BackgroundMediaPlayer.Current;
              currentSong.Pause();
              //do text to speech 
              Windows.Media.SpeechSynthesis.SpeechSynthesizer x = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
              var stream = await x.SynthesizeTextToStreamAsync(m.Description);
              var mediaElement = new MediaElement();
              mediaElement.SetSource(stream, stream.ContentType);
              mediaElement.Play();
              // resume song 
              currentSong.Play();

          }
          catch (Exception)
          {
              //debuger doesn't run voice
          }
      }
    }

    public void HideManeuver(ManeuverDescription m)
    {
      if (m != null && maneuvers.Contains(m))
      {
        maneuvers.Remove(m);
      }
    }

    public void Clear()
    {
        foreach (ManeuverDescription m in maneuvers) maneuvers.Remove(m);
        UpdateVisibility();
    }

    public void HideManeuver(string Id)
    {
      HideManeuver( maneuvers.Where(p => p.Id == Id).FirstOrDefault());
      UpdateVisibility();
    }

    private void UpdateVisibility()
    {
      this.Visibility = maneuvers.Any() ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateTile(string text)
    {
        // Create a notification for the Square310x310 tile using one of the available templates for the size.
        ITileSquare310x310Text09 tileContent = TileContentFactory.CreateTileSquare310x310Text09();
        tileContent.TextHeadingWrap.Text = text;
        
        // Create a notification for the Wide310x150 tile using one of the available templates for the size.
        ITileWide310x150Text03 wide310x150Content = TileContentFactory.CreateTileWide310x150Text03();
        wide310x150Content.TextHeadingWrap.Text = text;

        // Create a notification for the Square150x150 tile using one of the available templates for the size.
        ITileSquare150x150PeekImageAndText04 square150x150Content = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
        square150x150Content.TextBodyWrap.Text = text;
        square150x150Content.Image.Src = "Assets\\Logo.png";

        // Attach the Square150x150 template to the Wide310x150 template.
        wide310x150Content.Square150x150Content = square150x150Content;

        // Attach the Wide310x150 template to the Square310x310 template.
        tileContent.Wide310x150Content = wide310x150Content;

        
        // Send the notification to the application? tile.
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

    }
  }
}
