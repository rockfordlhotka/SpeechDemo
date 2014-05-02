using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SpeechDemo
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    public IReadOnlyList<VoiceInformation> VoiceList { get; set; }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var voices = SpeechSynthesizer.AllVoices;
      VoiceList = voices;
      this.DataContext = this;
      foreach (var item in voices)
        if (item.DisplayName == SpeechSynthesizer.DefaultVoice.DisplayName)
        {
          this.VoiceComboBox.SelectedItem = item;
          break;
        }
    }

    private async void SpeakText(object sender, RoutedEventArgs e)
    {
      string dialogText = null;
      try
      {
        var voice = this.VoiceComboBox.SelectedItem as VoiceInformation;
        if (voice != null)
        {
          var text = this.InputText.Text;
          if (!string.IsNullOrWhiteSpace(text))
          {
            var synthesizer = new SpeechSynthesizer();
            synthesizer.Voice = voice;
            var audioStream = await synthesizer.SynthesizeTextToStreamAsync(text);
            if (audioStream != null)
            {
              this.MediaPlayer.SetSource(audioStream, audioStream.ContentType);
              this.MediaPlayer.Play();
            }
            else
            {
              dialogText = "Can't synthesize the text";
            }
          }
        }
        return;
      }
      catch (Exception ex)
      {
        dialogText = "Error playing audio: " + ex.Message;
      }
      if (!string.IsNullOrWhiteSpace(dialogText))
        await new Windows.UI.Popups.MessageDialog(dialogText).ShowAsync();
    }
  }
}
