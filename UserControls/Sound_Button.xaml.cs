using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FiaMedKnuff.UserControls
{
    public sealed partial class Sound_Button : UserControl
    {
        public Sound_Button()
        {
            this.InitializeComponent();
        }

        private void SoundToggleButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.SoundEnabled = !SoundManager.SoundEnabled;

            if (SoundManager.SoundEnabled)
            {
                SoundImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/sound.png"));
            } else
            {
                SoundImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/sound-off.png"));
            }
        }
    }
}
