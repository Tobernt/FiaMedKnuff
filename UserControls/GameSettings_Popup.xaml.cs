using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using static FiaMedKnuff.Player;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FiaMedKnuff.UserControls
{
    public sealed partial class GameSettings_Popup : UserControl
    {
        public MainMenu MainMenuInstance { get; set; }
        private Dictionary<int, TextBlock> playerTextBlocks = new Dictionary<int, TextBlock>();
        private Dictionary<int, Player> players = new Dictionary<int, Player>();

        public GameSettings_Popup()
        {
            InitializeComponent();
            AddPlayerSelectors();
        }

        /// <summary>
        /// Creates 4 players
        /// </summary>
        private void AddPlayerSelectors()
        {
            for (int i = 0; i < 4; i++)
            {
                var playerGrid = CreatePlayerSelector(i);

                if (i == 0 || i == 1)
                {
                    Grid.SetRow(playerGrid, 1);
                    Grid.SetColumn(playerGrid, i == 0 ? 0 : 2);
                }
                else
                {
                    Grid.SetRow(playerGrid, 2);
                    Grid.SetColumn(playerGrid, i == 2 ? 2 : 0);
                }

                PlayerSelectorsGrid.Children.Add(playerGrid);
            }
        }

        /// <summary>
        /// Creates a player selector based on index and adds it to the grid
        /// </summary>
        private Grid CreatePlayerSelector(int playerNumber)
        {
            var outerGrid = new Grid
            {
                Margin = new Thickness(40),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            outerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            outerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });

            // Create inner grid for player icon
            var innerGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            innerGrid.Children.Add(new Rectangle
            {
                Width = 22,
                Height = 25,
                Fill = GetBodyColor(playerNumber),
                Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                StrokeThickness = 3,
                RadiusX = 3,
                RadiusY = 15,
                Margin = new Thickness(0, 15, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            });

            innerGrid.Children.Add(new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = GetHeadColor(playerNumber),
                Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                StrokeThickness = 3,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            });

            Grid.SetColumn(innerGrid, 1);
            outerGrid.Children.Add(innerGrid);

            // Create previous button
            var previousButton = new Button
            {
                Background = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0)),
                Content = "\u2190",
                CornerRadius = new CornerRadius(10),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5, 0, 0, 0)
            };
            previousButton.Click += (s, e) => OnPreviousButtonClick(playerNumber);
            Grid.SetColumn(previousButton, 0);
            Grid.SetRow(previousButton, 1);
            outerGrid.Children.Add(previousButton);

            // Create Player TextBlock
            var textBlock = new TextBlock
            {
                Text = $"Player {playerNumber + 1}",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 0, 0)
            };

            playerTextBlocks[playerNumber] = textBlock; // Save reference to dictionary

            Grid.SetColumn(textBlock, 1);
            Grid.SetRow(textBlock, 1);
            outerGrid.Children.Add(textBlock);

            // Create Next Button
            var nextButton = new Button
            {
                Background = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0)),
                CornerRadius = new CornerRadius(10),
                Content = "\u2192",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 5, 0)
            };
            nextButton.Click += (s, e) => OnNextButtonClick(playerNumber);
            Grid.SetColumn(nextButton, 2);
            Grid.SetRow(nextButton, 1);
            outerGrid.Children.Add(nextButton);

            // Add player to players array
            players[playerNumber] = new Player(Name = $"Player {playerNumber + 1}");
            players[playerNumber].Type = PlayerType.Player;

            return outerGrid;
        }

        /// <summary>
        /// Helper method to get headcolor based on index
        /// </summary>
        private SolidColorBrush GetHeadColor(int playerNumber)
        {
            Debug.WriteLine(playerNumber);
            switch (playerNumber)
            {
                case 0: return new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                case 1: return new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                case 2: return new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                case 3: return new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                default: return null;
            }
        }

        /// <summary>
        /// Helper method to get bodycolor based on index
        /// </summary>
        private SolidColorBrush GetBodyColor(int playerNumber)
        {
            Debug.WriteLine(playerNumber);
            switch (playerNumber)
            {
                case 0: return new SolidColorBrush(Color.FromArgb(255, 71, 0, 0)); //71,0,0,1.000
                case 1: return new SolidColorBrush(Color.FromArgb(255, 0, 33, 71));
                case 2: return new SolidColorBrush(Color.FromArgb(255, 0, 71, 2));
                case 3: return new SolidColorBrush(Color.FromArgb(255, 71, 60, 0));
                default: return null;
            }
        }

        private void OnPreviousButtonClick(int playerNumber)
        {
            // Get current player
            var player = players[playerNumber];

            // Get all player types
            var allTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));
            int currentIndex = Array.IndexOf(allTypes, player.Type);
            player.Type = (currentIndex > 0) ? allTypes[currentIndex - 1] : allTypes.Last();

            // Update the displayed text
            playerTextBlocks[playerNumber].Text = player.Type == PlayerType.Player ? player.Name : player.Type.ToString();
            Debug.WriteLine($"{player.Name} pressed previous");
        }

        private void OnNextButtonClick(int playerNumber)
        {
            // Get current player
            var player = players[playerNumber];

            // Get all player types
            var allTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));
            int currentIndex = Array.IndexOf(allTypes, player.Type);
            player.Type = (currentIndex < allTypes.Length - 1) ? allTypes[currentIndex + 1] : allTypes[0];

            // Update the displayed text
            playerTextBlocks[playerNumber].Text = player.Type == PlayerType.Player ? player.Name : player.Type.ToString();
            Debug.WriteLine($"{player.Name} pressed next");
        }

		// TODO: Go to MainPage window with players from dictionary
		private void Game_Start(object sender, RoutedEventArgs e)
		{
			if (MainMenuInstance != null && MainMenuInstance.gameSettingsPopup != null)
			{
				MainMenuInstance.gameSettingsPopup.Visibility = Visibility.Collapsed;
			}
			else
			{
				Debug.WriteLine("MainPageInstance or gameSettingsPopup is null");
			}

			// Collect the player types and pass them to the MainPage
			List<Player.PlayerType> playerTypes = players.Values.Select(p => p.Type).ToList();

			// Navigate to MainPage and pass the player types
			Frame navigationFrame = Window.Current.Content as Frame;
			navigationFrame.Navigate(typeof(MainPage), playerTypes); // Pass player types
		}
	}
}
