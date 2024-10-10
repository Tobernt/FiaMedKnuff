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
using Windows.UI.Xaml.Navigation;
using FiaMedKnuff;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FiaMedKnuff.UserControls
{
	public sealed partial class HighScore_Popup : UserControl
	{
		public MainMenu MainMenuInstance { get; set; }
		public HighScore_Popup()
		{
			this.InitializeComponent();
            PopulateHighScoreList();
        }

        private void Exit_Highscore_Btn(object sender, RoutedEventArgs e)
		{
			if (MainMenuInstance != null && MainMenuInstance.highScorePage != null)
			{
				MainMenuInstance.highScorePage.Visibility = Visibility.Collapsed;
			}
			else
			{
				// Handle null case or log the error
				Debug.WriteLine("MainPageInstance or highScorePage is null");
			}
		}

        public void PopulateHighScoreList()
        {
            Debug.WriteLine("Hello!");

            var highScores = PlayerScoreManager.LoadTopPlayerScores();

            Debug.WriteLine(highScores);

            foreach (var score in highScores)
            {
                // Create a horizontal StackPanel for each high score
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };

                // Create TextBlock for the player's name
                var nameTextBlock = new TextBlock
                {
                    Text = score.Name,
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    Margin = new Thickness(0, 0, 80, 0),
                    FontSize = 25
                };

                // Create TextBlock for the player's score
                var scoreTextBlock = new TextBlock
                {
                    Text = score.Moves.ToString(),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    FontSize = 25
                };

                // Add the TextBlocks to the StackPanel
                stackPanel.Children.Add(nameTextBlock);
                stackPanel.Children.Add(scoreTextBlock);

                // Add the StackPanel to the main StackPanel
                HighScoreList.Children.Add(stackPanel);
            }
        }

    }
}
