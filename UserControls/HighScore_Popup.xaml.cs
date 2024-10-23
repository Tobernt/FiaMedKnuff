using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

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
		public void StartPopupAnimation()
		{
            //Slide animation from bottom of page
			var slideInAnimation = new DoubleAnimation
			{
				From = 1000,
				To = 0,
				Duration = new Duration(TimeSpan.FromMilliseconds(600)),
				EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
			};

			var storyboard = new Storyboard();
			Storyboard.SetTarget(slideInAnimation, PopupTranslateTransform);
			Storyboard.SetTargetProperty(slideInAnimation, "Y");

			storyboard.Children.Add(slideInAnimation);
			storyboard.Begin();
		}
		public void SlideOutAnimation(EventHandler<object> completedCallback = null)
		{
			//Slide animation to bottom of page, before collapsed
			var slideOutAnimation = new DoubleAnimation
			{
				From = 0,
				To = 1000,
				Duration = new Duration(TimeSpan.FromMilliseconds(600)),
				EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
			};

			var storyboard = new Storyboard();
			Storyboard.SetTarget(slideOutAnimation, PopupTranslateTransform);
			Storyboard.SetTargetProperty(slideOutAnimation, "Y");
			if (completedCallback != null)
			{
				storyboard.Completed += completedCallback;
			}

			storyboard.Children.Add(slideOutAnimation);
			storyboard.Begin();
		}
		private void Exit_Highscore_Btn(object sender, RoutedEventArgs e)
		{
            SlideOutAnimation((s, args) => MainMenuInstance.highScorePage.Visibility = Visibility.Collapsed);
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
