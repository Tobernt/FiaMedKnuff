using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuff
{
    public sealed partial class MainMenu : Page
    {
        public MainMenu()
        {
            this.InitializeComponent();

            // Initialize highScorePage and gameSettingsPopup if available
            if (highScorePage != null)
            {
                highScorePage.MainMenuInstance = this;
            }

            if (gameSettingsPopup != null)
            {
                gameSettingsPopup.MainMenuInstance = this;
            }

            // Initially hide the back button
            BackButton.Visibility = Visibility.Collapsed;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Show the game settings popup for player selection
            gameSettingsPopup.Visibility = Visibility.Visible;

            // Show the back button to allow returning to the main menu
            BackButton.Visibility = Visibility.Visible;
        }

        private void HighScore_Click(object sender, RoutedEventArgs e)
        {
            // Popup for highscore
			highScorePage.Visibility = Visibility.Visible;
            highScorePage.StartPopupAnimation();
		}
        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            // Implement rules popup or logic if needed
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            // Quit the application
            CoreApplication.Exit();
        }

        // Back button event handler to return from settings to main menu
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the game settings popup
            gameSettingsPopup.Visibility = Visibility.Collapsed;

            // Hide the back button since we're back to the main menu
            BackButton.Visibility = Visibility.Collapsed;
        }
    }
}
