using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuff
{
    public sealed partial class MainMenu : Page
    {
        public MainMenu()
        {
            this.InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the MainPage
            Frame.Navigate(typeof(MainPage));
        }

        private void HighScore_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Rules_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Quit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}