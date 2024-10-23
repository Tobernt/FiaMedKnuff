using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using FiaMedKnuff;
using Windows.Media.PlayTo;
using Windows.UI.Xaml.Input;
using Windows.UI;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        private Player[] players; // use gamelogic class later
        private int currentPlayerIndex;
        private int totalPlayers = 4;
        private Random random;
        DateTime startTime = DateTime.Now;
        private int selectedTokenIndex = -1;
        private int diceRoll;

        // Paths for each player, defined as (row, column) positions on the grid.
        private readonly (int row, int col)[] RedPath = new (int row, int col)[]
        {
            // Starting from Red quadrant at (4, 0)
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5), (0, 6),
            (1, 6), (2, 6), (3, 6), (4, 6), (4, 7), (4, 8),
            (4, 9), (4, 10), (5, 10), (6, 10), (6, 9), (6, 8),
            (6, 7), (6, 6), (7, 6), (8, 6), (9, 6), (10, 6),
            (10, 5), (10, 4), (9, 4), (8, 4), (7, 4), (6, 4),
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), (5, 1),
            (5, 2), (5,3),(5,4),(5,5)
        };

        private readonly (int row, int col)[] BluePath = new (int row, int col)[]
        {
            // Starting from Blue quadrant at (0, 6), moving down
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6),
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10),
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6),
            (7, 6), (8, 6), (9, 6), (10, 6), (10, 5),
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4),
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0),
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5),
            (1,5), (2,5), (3,5), (4,5), (5,5)
        };

        private readonly (int row, int col)[] YellowPath = new (int row, int col)[]
        {
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4),
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0),
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5),
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6),
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10),
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6),
            (7, 6), (8, 6), (9, 6), (10, 6), (10,5),
            (9,5), (8,5),(7, 5), (6,5),(5,5)
        };

        private readonly (int row, int col)[] GreenPath = new (int row, int col)[]
        {
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6),
            (7, 6), (8, 6), (9, 6), (10, 6), (10, 5),
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4),
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0),
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5),
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6),
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10),
            (5, 9), (5, 8), (5, 7),(5, 6), (5, 5)
        };


        public MainPage()
		{
			this.InitializeComponent();
			random = new Random();
		}

		public MainPage(List<Player.PlayerType> playerTypes)
		{
			this.InitializeComponent();
			random = new Random();

			// Initialize players using the passed player types
			players = new Player[]
			{
		        new Player("red") { Type = playerTypes[0] },
		        new Player("blue") { Type = playerTypes[1] },
		        new Player("green") { Type = playerTypes[2] },
		        new Player("yellow") { Type = playerTypes[3] }
			};

			currentPlayerIndex = random.Next(0, 4);
			DiceIsEnable(currentPlayerIndex);  // Continue with game logic
		}
        private void DiceIsEnable(int currentPlayerIndex)
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, YellowDiceBtn, GreenDiceBtn };
            while (players[currentPlayerIndex].Type == Player.PlayerType.None)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            }
            foreach (Button button in diceButtons)
            {
                button.IsEnabled = false;
                button.Visibility = Visibility.Collapsed;
            }
            diceButtons[currentPlayerIndex].IsEnabled = false;
            diceButtons[currentPlayerIndex].Visibility = Visibility.Visible;

            if (currentPlayerIndex >= 0 && currentPlayerIndex < diceButtons.Length)
            {
                diceButtons[currentPlayerIndex].IsEnabled = true;
            }

            // Disable token selection until dice is rolled
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);
                    token.IsTapEnabled = false; // Disable tapping on tokens initially
                }
            }
        }
        private async void RollDice_Click(object sender, RoutedEventArgs e)
        {
            DeselectAllTokens();
            // Ensure that the current player is valid
            while (players[currentPlayerIndex].Type == Player.PlayerType.None)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            }

            // Disable dice after rolling to prevent multiple rolls
            DisableDiceForCurrentPlayer();

            diceRoll = RollDice();

            // Display the result of the dice roll
            Button clickedButton = sender as Button;
            if (clickedButton == RedDiceBtn) RedDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == BlueDiceBtn) BlueDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == GreenDiceBtn) GreenDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == YellowDiceBtn) YellowDice.ThrowDiceVisual(diceRoll);

            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} rolled a {diceRoll}";

            bool hasPiecesOnBoard = players[currentPlayerIndex].HasPiecesOnBoard;
            bool hasPiecesInNest = players[currentPlayerIndex].HasPiecesInNest();

            // Automatically pass turn if all pieces are in the nest and dice roll isn't 1 or 6
            if (hasPiecesInNest && !hasPiecesOnBoard && diceRoll != 1 && diceRoll != 6)
            {
                PassTurnToNextPlayer();
                return;
            }

            // Handle roll of 6 with dialog options
            if (diceRoll == 6)
            {
                if (hasPiecesInNest || hasPiecesOnBoard)
                {
                    // Using radio buttons in the content dialog for the three options
                    StackPanel contentPanel = new StackPanel();
                    RadioButton moveOneTokenOut = new RadioButton { Content = "Move 1 token out 6 steps", GroupName = "Options", IsChecked = true };
                    RadioButton moveTwoTokensOut = new RadioButton { Content = "Move 2 tokens 1 step each", GroupName = "Options" };
                    RadioButton moveOnBoardToken = new RadioButton { Content = "Move a token on the board 6 steps", GroupName = "Options" };

                    contentPanel.Children.Add(moveOneTokenOut);
                    contentPanel.Children.Add(moveTwoTokensOut);
                    contentPanel.Children.Add(moveOnBoardToken);

                    ContentDialog choiceDialog = new ContentDialog
                    {
                        Title = "Move Choice",
                        Content = contentPanel,
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel"
                    };

                    ContentDialogResult result = await choiceDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        if (moveOneTokenOut.IsChecked == true)
                        {
                            // Move 1 token out 6 steps
                            int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                            players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                            MovePlayer(currentPlayerIndex, 5, tokenToMoveOut);

                            // Disable token selection and allow rolling again
                            DisableTokenSelection();  // Disable further token movement after moving out
                            PassTurnOrEnableRollForSix();  // Check if they should roll again
                        }
                        else if (moveTwoTokensOut.IsChecked == true)
                        {
                            // Move 2 tokens out 1 step each
                            int firstToken = GetNextTokenInNest(currentPlayerIndex);
                            players[currentPlayerIndex].MoveOutOfNest(firstToken);
                            MovePlayer(currentPlayerIndex, 0, firstToken);
                            DisableTokenSelection();
                            if (players[currentPlayerIndex].HasPiecesInNest())
                            {
                                int secondToken = GetNextTokenInNest(currentPlayerIndex);
                                players[currentPlayerIndex].MoveOutOfNest(secondToken);
                                MovePlayer(currentPlayerIndex, 0, secondToken);
                                DisableTokenSelection();
                            }

                            // Disable token selection and allow rolling again
                            DisableTokenSelection();  // Disable further token movement after moving out
                            PassTurnOrEnableRollForSix();  // Check if they should roll again
                        }
                        else if (moveOnBoardToken.IsChecked == true)
                        {
                            // Enable selection for moving a token on the board 6 steps
                            EnableTokenSelectionForSixSteps(currentPlayerIndex);
                            return;  // Wait for token selection
                        }
                    }
                }
                else
                {
                    // If only pieces on board, allow moving one of them 6 steps
                    EnableTokenSelectionForSixSteps(currentPlayerIndex);
                    return;  // Wait for token selection, don't pass the turn yet
                }
            }
            else if (diceRoll == 1)
            {
                // Handle roll of 1 in a similar fashion
                if (hasPiecesInNest && hasPiecesOnBoard)
                {
                    StackPanel contentPanel = new StackPanel();
                    RadioButton moveOneTokenOut = new RadioButton { Content = "Move 1 token out 1 step", GroupName = "Options", IsChecked = true };
                    RadioButton moveOnBoardToken = new RadioButton { Content = "Move a token on the board 1 step", GroupName = "Options" };

                    contentPanel.Children.Add(moveOneTokenOut);
                    contentPanel.Children.Add(moveOnBoardToken);

                    ContentDialog choiceDialog = new ContentDialog
                    {
                        Title = "Move Choice",
                        Content = contentPanel,
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel"
                    };

                    ContentDialogResult result = await choiceDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        if (moveOneTokenOut.IsChecked == true)
                        {
                            int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                            players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                            MovePlayer(currentPlayerIndex, 0, tokenToMoveOut);

                            DisableTokenSelection();  // Disable further token movement
                            PassTurnOrEnableRollForSix();  // Check if they should roll again or pass
                        }
                        else if (moveOnBoardToken.IsChecked == true)
                        {
                            EnableTokenSelectionForOneStep(currentPlayerIndex);  // Allow moving a token on the board by 1 step
                            return;  // Wait for token selection, don't pass the turn yet
                        }
                    }
                }
                else if (hasPiecesInNest && !hasPiecesOnBoard)
                {
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                    MovePlayer(currentPlayerIndex, 0, tokenToMoveOut);

                    DisableTokenSelection();  // Disable further token movement
                    PassTurnOrEnableRollForSix();  // Check if they should roll again or pass
                }
                else if (!hasPiecesInNest && hasPiecesOnBoard)
                {
                    EnableTokenSelectionForOneStep(currentPlayerIndex);  // Allow moving a token on the board by 1 step
                    return;  // Wait for token selection, don't pass the turn yet
                }
            }
            else
            {
                EnableTokenSelection(currentPlayerIndex);  // Enable selecting a token to move after rolling
            }
        }

        private void PassTurnOrEnableRollForSix()
        {
            if (diceRoll == 6)
            {
                // Allow player to roll again after rolling a 6
                DiceRollResult.Text += " (Player gets to roll again!)";
                EnableDiceForCurrentPlayer(); // Allow rolling again
            }
            else
            {
                diceRoll = 0;  // Reset dice roll if not 6
                PassTurnToNextPlayer(); // Pass turn to the next player
            }
        }

        private void EnableTokenSelection(int playerIndex)
        {
            // Make the current player's tokens selectable
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                // Allow selection of tokens in the nest (-1) ONLY if a 1 or 6 is rolled, otherwise skip nest tokens
                if (tokenPosition >= 0 || (tokenPosition == -1 && (diceRoll == 1 || diceRoll == 6)))
                {
                    token.IsTapEnabled = true;  // Make the token tappable if valid
                }
                else
                {
                    token.IsTapEnabled = false; // Ensure nest tokens are not tappable unless 1 or 6 is rolled
                }
            }

            // Prompt the player to choose a token
            DiceRollResult.Text = $"Select a token to move.";
        }

        private void EnableTokenSelectionForSixSteps(int playerIndex)
        {
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                // Only enable pieces already on the board (position >= 0) for moving 6 steps
                if (tokenPosition >= 0 && tokenPosition < 99)
                {
                    token.IsTapEnabled = true;
                }
            }

            // Prompt the player to choose a token to move 6 steps
            DiceRollResult.Text = $"Select a token on the board to move 6 steps.";
        }


        private void EnableTokenSelectionForOneStep(int playerIndex)
        {
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                // Only enable pieces already on the board (position >= 0) for moving 1 step
                if (tokenPosition >= 0 && tokenPosition < 99)
                {
                    token.IsTapEnabled = true;
                }
                else
                {
                    token.IsTapEnabled = false; // Ensure that tokens in the nest aren't selectable
                }
            }

            // Prompt the player to choose a token to move 1 step
            DiceRollResult.Text = $"Select a token on the board to move 1 step.";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			// Ensure we received player types from the game settings
			if (e.Parameter is List<Player.PlayerType> playerTypes)
			{
				// Call the overloaded constructor to initialize players with the correct types
				players = new Player[]
				{
			        new Player("red") { Type = playerTypes[0] },
			        new Player("blue") { Type = playerTypes[1] },
			        new Player("green") { Type = playerTypes[2] },
			        new Player("yellow") { Type = playerTypes[3] }
				};

				currentPlayerIndex = random.Next(0, 4);
				DiceIsEnable(currentPlayerIndex);
			}
			for (int i = 0; i < players.Length; i++)
			{
				Debug.WriteLine($"MainPage - Player {i + 1} type: {players[i].Type}");
			}
		}

        //Method applying new strokethickness to tapped token
        private void HighlightSelectedToken(Grid tokenGrid)
        {
            AddDropShadow(tokenGrid);

            foreach (var child in tokenGrid.Children)
            {
                if (child is Rectangle rectangle)
                {
                    rectangle.StrokeThickness = 5;
                }

                else if (child is Ellipse ellipse)
                {
                    ellipse.StrokeThickness = 5;
                }
            }
        }

        //Method applying shadoweffect to tapped token
        private void AddDropShadow(Grid targetElement)
        {
            var compositor = Window.Current.Compositor;

            var dropShadow = compositor.CreateDropShadow();

            dropShadow.Color = Colors.Black;
            dropShadow.BlurRadius = 15;
            dropShadow.Opacity = (float)0.4;

            var shadowVisual = compositor.CreateSpriteVisual();
            shadowVisual.Size = new System.Numerics.Vector2((float)targetElement.ActualWidth, (float)targetElement.ActualHeight);
            shadowVisual.Shadow = dropShadow;

            if (targetElement.Children[1] is Ellipse ellipse)
            {
                dropShadow.Mask = ellipse.GetAlphaMask();
            }

            else if (targetElement.Children[0] is Rectangle rectangle)
            {
                dropShadow.Mask = rectangle.GetAlphaMask();
            }

            ElementCompositionPreview.SetElementChildVisual(targetElement, shadowVisual);
        }

        //Method removing added effects to tapped token
        private void ResetTokenEffects(Grid tokenGrid)
        {
            ElementCompositionPreview.SetElementChildVisual(tokenGrid, null);

            foreach (var child in tokenGrid.Children)
            {
                if (child is Rectangle rectangle)
                {
                    rectangle.StrokeThickness = 3;
                }

                else if (child is Ellipse ellipse)
                {
                    ellipse.StrokeThickness = 3;
                }
            }
        }

        private void Chosen_Token(object sender, TappedRoutedEventArgs e)
        {
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);

                    token.Tapped += OnTokenTapped;
                }
            }
        }

        private void OnTokenTapped(object sender, TappedRoutedEventArgs e)
        {
            Grid clickedToken = sender as Grid;
            // Avmarkera alla pjäser först
            DeselectAllTokens();

            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);

                    if (token == clickedToken && playerIndex == currentPlayerIndex)
                    {
                        int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                        // If the token is in the nest and the player rolled a 1 or 6, move it out of the nest
                        if (tokenPosition == -1 && (diceRoll == 1 || diceRoll == 6))
                        {
                            players[currentPlayerIndex].MoveOutOfNest(tokenIndex);
                            MovePlayer(currentPlayerIndex, 0, tokenIndex);  // Move to the start position
                            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} moved a token out of the nest!";
                        }
                        // If the token is on the board, move it based on the dice roll
                        else if (tokenPosition >= 0 && tokenPosition != 99)
                        {
                            MovePlayer(currentPlayerIndex, diceRoll, tokenIndex);
                            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} moved a token!";
                        }

                        // Disable token selection after a move
                        selectedTokenIndex = -1;
                        DisableTokenSelection();

                        // Reset the dice roll value after a move
                        diceRoll = 0;

                        // If the player rolled a 6, allow reroll after moving
                        if (diceRoll == 6)
                        {
                            DiceRollResult.Text += " (Player gets to roll again!)";
                            EnableDiceForCurrentPlayer(); // Allow reroll
                        }
                        else
                        {
                            PassTurnToNextPlayer(); // Pass the turn if it's not a 6
                        }

                        return; // Exit after handling the token tap
                    }
                    HighlightSelectedToken(clickedToken); // Markera den valda pjäsen
                }
            }
        }
        private void DeselectAllTokens()
        {
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);

                    // Återställ visuella effekter (ta bort highlight)
                    ResetTokenEffects(token);
                }
            }

            // Nollställ den markerade pjäsen
            selectedTokenIndex = -1;
        }
        private void PassTurnToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            diceRoll = 0;  // Reset the dice roll here
            DiceIsEnable(currentPlayerIndex); // Enable the next player's dice
        }


        private void PassTurnIfNeeded(bool hasMoved)
        {
            // Check if all pieces are in the nest
            bool allPiecesInNest = players[currentPlayerIndex].HasPiecesInNest();

            // If the player has moved, or all pieces are in the nest and they didn't roll 1 or 6, pass the turn
            if (hasMoved || (allPiecesInNest && (diceRoll != 1 && diceRoll != 6)))
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
                diceRoll = 0;  // Reset the dice roll here
                DiceIsEnable(currentPlayerIndex); // Enable the next player's dice
            }
            else
            {
                // If the player cannot move (all pieces in nest) and rolled 1 or 6, they get another chance
                DiceIsEnable(currentPlayerIndex); // Keep the current player's turn
            }
        }


        private void DisableTokenSelection()
        {
            // Disable tapping on tokens for all players
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);
                    token.IsTapEnabled = false;
                }
            }
        }

        private void DisableDiceForCurrentPlayer()
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
            diceButtons[currentPlayerIndex].IsEnabled = false;
        }

        private void EnableDiceForCurrentPlayer()
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
            diceButtons[currentPlayerIndex].IsEnabled = true;
        }

        private int RollDice()
        {
            return random.Next(1, 7);
        }
        private void MovePlayer(int playerIndex, int steps, int tokenIndex)
        {
            int currentPosition = players[playerIndex].GetTokenPosition(tokenIndex);

            // Om pjäsen redan är i mål, gör inget
            if (currentPosition == 99)
            {
                return;
            }

            // Flytta pjäsen från boet till start om den fortfarande är i boet
            if (currentPosition == -1)
            {
                players[playerIndex].SetTokenPosition(tokenIndex, 0);
                players[playerIndex].PiecesInNest--;
            }
            else if (currentPosition + steps > GetPlayerPath(playerIndex).Length - 1)
            {
                // Om kastet gör att pjäsen går förbi målet, flytta tillbaka överflödiga steg
                var path = GetPlayerPath(playerIndex);
                int moveBack = PacesToMoveBack(currentPosition, path.Length, steps);
                int newPositionOnBoard = currentPosition - moveBack;
                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);
                Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
            }
            else
            {
                // Flytta pjäsen framåt med antal steg
                int newPositionOnBoard = currentPosition + steps;
                var path = GetPlayerPath(playerIndex);

                if (newPositionOnBoard >= path.Length)
                {
                    newPositionOnBoard = path.Length - 1;
                }

                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);

                // Kontrollera om pjäsen nått målet
                if (newPositionOnBoard == path.Length - 1)
                {
                    players[playerIndex].SetTokenPosition(tokenIndex, 99); // Markera att pjäsen är i mål
                    HandlePlayerGoal(playerIndex, tokenIndex); // Kontrollera om spelaren har vunnit
                }

                // Flytta pjäsen visuellt
                Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
                CheckForOverlappingTokens(playerIndex, tokenIndex);

                ResetTokenEffects(playerToken);
            }
        }
        private void CheckForOverlappingTokens(int playerIndex, int tokenIndex)
        {
            // Get the Grid.Row and Grid.Column of the moved token
            Grid movedTokenGrid = GetPlayerToken(playerIndex, tokenIndex);
            int movedTokenRow = Grid.GetRow(movedTokenGrid);
            int movedTokenCol = Grid.GetColumn(movedTokenGrid);

            // Loop through all other players to check if any token is on the same grid location
            for (int otherPlayerIndex = 0; otherPlayerIndex < players.Length; otherPlayerIndex++)
            {
                if (otherPlayerIndex == playerIndex) continue; // Skip checking the current player's own tokens

                // Loop through the tokens of the other player
                for (int otherTokenIndex = 0; otherTokenIndex < 4; otherTokenIndex++)
                {
                    Grid otherTokenGrid = GetPlayerToken(otherPlayerIndex, otherTokenIndex);
                    int otherTokenRow = Grid.GetRow(otherTokenGrid);
                    int otherTokenCol = Grid.GetColumn(otherTokenGrid);

                    // Compare the grid positions of both tokens
                    if (movedTokenRow == otherTokenRow && movedTokenCol == otherTokenCol)
                    {
                        // Exclude the goal position (5, 5) from knockouts
                        if (movedTokenRow == 5 && movedTokenCol == 5)
                        {
                            continue; // Skip knockout for tokens in the goal position
                        }

                        // Push the other player's token back to the nest
                        players[otherPlayerIndex].SetTokenPosition(otherTokenIndex, -1); // -1 means back to the nest
                        players[otherPlayerIndex].PiecesInNest++; // Increment the opponent's PiecesInNest count

                        // Remove the token from the board visually
                        RepopulateNest(otherPlayerIndex, otherTokenIndex);

                        // Optionally, display a message about the knockout
                        DiceRollResult.Text = $"{IndexToName(playerIndex)} knocked out {IndexToName(otherPlayerIndex)}'s piece!";

                        // Break out after knocking out one piece
                        return;
                    }
                }
            }
        }
        private void RepopulateNest(int playerIndex, int tokenIndex)
        {
            // Get the player token visually
            Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);

            // Use the existing nest coordinates from the player grid positions
            switch (playerIndex)
            {
                case 0: // Red player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 0, 0);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 0, 1);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 1, 0);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 1, 1);
                    break;

                case 1: // Blue player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 0, 9);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 0, 10);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 1, 9);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 1, 10);
                    break;

                case 2: // Green player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 9, 9);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 9, 10);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 10, 9);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 10, 10);
                    break;

                case 3: // Yellow player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 9, 0);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 9, 1);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 10, 0);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 10, 1);
                    break;
            }
        }



        private int GetNextTokenInNest(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[playerIndex].GetTokenPosition(i) == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetNextTokenOnBoard(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                int tokenPosition = players[playerIndex].GetTokenPosition(i);
                if (tokenPosition != -1 && tokenPosition != 99)
                {
                    return i;
                }
            }
            return -1;
        }

		private int PacesToMoveBack(int position, int pathLength, int steps)
		{
			int pacesToGoal = (pathLength - 1) - position;
			int moveBackPaces = (position + steps) - (pathLength - 1);
			return moveBackPaces - pacesToGoal; // Returns the amount of paces to go back if larger than paces to goal
		}
        private void HandlePlayerGoal(int playerIndex, int tokenIndex)
        {
            // Hämta färgnamnet för spelaren
            string playerColor = IndexToName(playerIndex);

            // Uppdatera resultattexten för att visa att spelaren nått målet
            DiceRollResult.Text = $"Player {playerColor} has reached the goal with one of their pieces!";

            // Markera pjäsen som i mål genom att sätta positionen till 99
            players[playerIndex].SetTokenPosition(tokenIndex, 99);

            // Kontrollera om alla pjäser för spelaren är i mål
            if (players[playerIndex].AllPiecesInGoal())
            {
                // Om alla pjäser är i mål, markera att spelaren har vunnit
                players[playerIndex].HasWon = true;

                // Uppdatera resultattexten för att visa att spelaren har vunnit spelet
                DiceRollResult.Text += $" {playerColor} has won the game!";

                // Här kan du lägga till logik för att avsluta spelet eller visa ett popup-meddelande om vinsten
                // Exempelvis kan du lägga till en ContentDialog för att meddela att spelet är över
                var winDialog = new ContentDialog
                {
                    Title = "Game Over",
                    Content = $"Congratulations! {playerColor} has won the game!",
                    CloseButtonText = "OK"
                };

                // Visa vinstdialogen
                _ = winDialog.ShowAsync();
            }
        }


        private string IndexToName(int index)
        {
            switch (index)
            {
                case 0: return "Red";
                case 1: return "Blue";
                case 2: return "Green";
                case 3: return "Yellow";
                default: return null;
            }
        }

        private Grid GetPlayerToken(int playerIndex, int tokenIndex)
        {
            switch (playerIndex)
            {
                case 0:
                    switch (tokenIndex)
                    {
                        case 0: return Player1Token;
                        case 1: return Player1Token2;
                        case 2: return Player1Token3;
                        case 3: return Player1Token4;
                    }
                    break;
                case 1:
                    switch (tokenIndex)
                    {
                        case 0: return Player2Token;
                        case 1: return Player2Token2;
                        case 2: return Player2Token3;
                        case 3: return Player2Token4;
                    }
                    break;
                case 2:
                    switch (tokenIndex)
                    {
                        case 0: return Player3Token;
                        case 1: return Player3Token2;
                        case 2: return Player3Token3;
                        case 3: return Player3Token4;
                    }
                    break;
                case 3:
                    switch (tokenIndex)
                    {
                        case 0: return Player4Token;
                        case 1: return Player4Token2;
                        case 2: return Player4Token3;
                        case 3: return Player4Token4;
                    }
                    break;
            }
            return null;
        }

        private (int row, int col)[] GetPlayerPath(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0: return RedPath;
                case 1: return BluePath;
                case 2: return GreenPath;
                case 3: return YellowPath;
                default: return null;
            }
        }

        private void SetTokenPosition(Grid token, int row, int col)
        {
            Grid.SetRow(token, row);
            Grid.SetColumn(token, col);
        }

        private void Back_to_MainMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }
    }
}
