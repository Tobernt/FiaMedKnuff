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
                // Create a Grid for each high score entry
                var grid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 10, 0, 10) // Equal vertical spacing between rows
                };

                // Define three columns for Name, Score, and Time
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Fixed width for Name
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // Fixed width for Score
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Fixed width for Time

                // Create TextBlock for the player's name
                var nameTextBlock = new TextBlock
                {
                    Text = score.Name,
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    FontSize = 25,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                // Create TextBlock for the player's score
                var scoreTextBlock = new TextBlock
                {
                    Text = score.Moves.ToString(),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    FontSize = 25,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Create TextBlock for the player's time
                var timeTextBlock = new TextBlock
                {
                    Text = score.Time,
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    FontSize = 25,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Add the TextBlocks to the Grid
                Grid.SetColumn(nameTextBlock, 0); // Name in first column
                Grid.SetColumn(scoreTextBlock, 1); // Score in second column
                Grid.SetColumn(timeTextBlock, 2); // Time in third column

                grid.Children.Add(nameTextBlock);
                grid.Children.Add(scoreTextBlock);
                grid.Children.Add(timeTextBlock);

                // Add the Grid to the main StackPanel
                HighScoreList.Children.Add(grid);
            }
        }
    }
}
