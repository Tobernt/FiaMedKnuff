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
using Windows.UI.Xaml.Media.Animation;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        private Player[] players;//Use gamelogic class later
        private int currentPlayerIndex;
        private int totalPlayers = 4;
        private Random random;
        DateTime startTime = DateTime.Now;
        private int selectedTokenIndex = -1;
        private int diceRoll;
        private bool stopGame = false;

        //Paths for each player, defined as (row, column) positions on the grid.
        private readonly (int row, int col)[] RedPath = new (int row, int col)[]
        {
            //Starting from Red quadrant at (4, 0)
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
            //Starting from Blue quadrant at (0, 6), moving down
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

        private readonly (int row, int col)[] GreenPath = new (int row, int col)[]
        {
            //Starting from Green quadrant at (6, 10), moving down
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

        private readonly (int row, int col)[] YellowPath = new (int row, int col)[]
        {
            //Starting from Yellow quadrant at (10, 4), moving down
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

        public MainPage()
		{
			this.InitializeComponent();
			random = new Random();
        
		}

        /// <summary>
        /// Enables the dice button for the current player and disables the rest.
        /// Hides all dice buttons initially and ensures that the appropriate button
        /// is visible for the current player. Also disables token selection until
        /// the dice is rolled.
        /// </summary>
        /// <param name="currentPlayerIndex">The index of the current player whose dice button should be enabled.</param>
        private void DiceIsEnable(int currentPlayerIndex)
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
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

            //Disable token selection until dice is rolled
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);
                    token.IsTapEnabled = false;//Disable tapping on tokens initially
                }
            }
		}

        /// <summary>
        /// Handles the turn for a computer player, including rolling the dice, 
        /// moving pieces based on the rolled value, and managing turn transitions 
        /// between players. It recursively handles consecutive turns for computer players.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleComputerTurn()
        {
            var player = players[currentPlayerIndex];

            if (stopGame)
                return;

            //Add some delay for piece_place sound to finish 
            await Task.Delay(200);

            //Runs atleast once
            do
            {
                bool hasPiecesOnBoard = player.HasPiecesOnBoard;
                bool hasPiecesInNest = player.HasPiecesInNest();
                bool allPiecesInNestOrGoal = player.AllPiecesInNestOrGoal();

                //Roll dice for ai
                diceRoll = RollDice();

                //Play dice sound
                SoundManager.PlaySound(SoundType.DiceRoll);

                if (currentPlayerIndex == 0) RedDice.ThrowDiceVisual(diceRoll);
                if (currentPlayerIndex == 1) BlueDice.ThrowDiceVisual(diceRoll);
                if (currentPlayerIndex == 2) GreenDice.ThrowDiceVisual(diceRoll);
                if (currentPlayerIndex == 3) YellowDice.ThrowDiceVisual(diceRoll);

                Debug.WriteLine("Computer rolled " + diceRoll);

                await Task.Delay(800);

                DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} rolled a {diceRoll}";

                //Logic for moving out of the nest
                if (diceRoll == 1 && hasPiecesInNest)
                {
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    player.MoveOutOfNest(tokenToMoveOut);
                    MovePlayer(currentPlayerIndex, 0, tokenToMoveOut, GetPlayerToken(currentPlayerIndex, tokenToMoveOut));

                }
                else if (diceRoll == 6 && hasPiecesInNest)
                {
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    player.MoveOutOfNest(tokenToMoveOut);
                    MovePlayer(currentPlayerIndex, 5, tokenToMoveOut, GetPlayerToken(currentPlayerIndex, tokenToMoveOut));

                }
                else if (hasPiecesOnBoard && !allPiecesInNestOrGoal)
                {
                    int tokenOnBoard = GetNextTokenOnBoard(currentPlayerIndex);
                    MovePlayer(currentPlayerIndex, diceRoll, tokenOnBoard, GetPlayerToken(currentPlayerIndex, tokenOnBoard));

                }

                if (diceRoll != 6)
                {
                    break;
                }

                DiceRollResult.Text += $" ({IndexToName(currentPlayerIndex)} gets to roll again!)";

                await Task.Delay(1000);
            }
            while (diceRoll == 6);

            //Pass the turn to the next player after computer finishes
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;

            //Skip None-type players
            while (players[currentPlayerIndex].Type == Player.PlayerType.None)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
                Debug.WriteLine($"Skipped {players[currentPlayerIndex].Name} because type is {players[currentPlayerIndex].Type}.");
            }

            DiceIsEnable(currentPlayerIndex);

            //If the next player is a computer, handle their turn
            if (players[currentPlayerIndex].Type == Player.PlayerType.Computer)
            {
                Debug.WriteLine("Next player is Computer, handling their turn.");
                await HandleComputerTurn();
            }
        }

        /// <summary>
        /// Handles the event when the dice button is clicked, including rolling the dice, 
        /// updating player states, and managing token movement based on the rolled value.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private async void RollDice_Click(object sender, RoutedEventArgs e)
        {
            DeselectAllTokens();
            //Ensure that the current player is valid
            while (players[currentPlayerIndex].Type == Player.PlayerType.None)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            }
            bool hasPiecesOnBoardLocal = players[currentPlayerIndex].HasPiecesOnBoard;

            //Check if the current player is a computer and handle their turn 
            if (players[currentPlayerIndex].Type == Player.PlayerType.Computer)
            {
                Debug.WriteLine("Current player is Computer, handling their turn.");
                await HandleComputerTurn();
                return;
            }

            //Disable dice after rolling to prevent multiple rolls
            DisableDiceForCurrentPlayer();
            if (hasPiecesOnBoardLocal && selectedTokenIndex == -1)
            {
                DiceRollResult.Text = $"(Click {IndexToName(currentPlayerIndex)} token to move)";
                SoundManager.PlaySound(SoundType.Error);
            }

            diceRoll = RollDice();

            SoundManager.PlaySound(SoundType.DiceRoll);
            //Display the result of the dice roll
            Button clickedButton = sender as Button;
            if (clickedButton == RedDiceBtn) RedDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == BlueDiceBtn) BlueDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == GreenDiceBtn) GreenDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == YellowDiceBtn) YellowDice.ThrowDiceVisual(diceRoll);

            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} rolled a {diceRoll}";

            bool hasPiecesOnBoard = players[currentPlayerIndex].HasPiecesOnBoard;
            bool hasPiecesInNest = players[currentPlayerIndex].HasPiecesInNest();

            //Automatically pass turn if all pieces are in the nest and dice roll isn't 1 or 6
            if (hasPiecesInNest && !hasPiecesOnBoard && diceRoll != 1 && diceRoll != 6)
            {
                PassTurnToNextPlayer();
                return;
            }

            //Handle roll of 6 with dialog options
            if (diceRoll == 6)
            {
                if (hasPiecesInNest || hasPiecesOnBoard)
                {
                    //Using radio buttons in the content dialog for the three options
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
                            //Move 1 token out 6 steps
                            int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                            players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                            AnimateToken(currentPlayerIndex, 5, tokenToMoveOut);

                            //Disable token selection and allow rolling again
                            DisableTokenSelection();//Disable further token movement after moving out
                            PassTurnOrEnableRollForSix();//Check if they should roll again
                        }
                        else if (moveTwoTokensOut.IsChecked == true)
                        {
                            //Move 2 tokens out 1 step each
                            int firstToken = GetNextTokenInNest(currentPlayerIndex);
                            players[currentPlayerIndex].MoveOutOfNest(firstToken);
                            AnimateToken(currentPlayerIndex, 0, firstToken);
                            DisableTokenSelection();
                            if (players[currentPlayerIndex].HasPiecesInNest())
                            {
                                int secondToken = GetNextTokenInNest(currentPlayerIndex);
                                players[currentPlayerIndex].MoveOutOfNest(secondToken);
                                AnimateToken(currentPlayerIndex, 0, secondToken);
                                DisableTokenSelection();
                            }

                            //Disable token selection and allow rolling again
                            DisableTokenSelection();//Disable further token movement after moving out
                            PassTurnOrEnableRollForSix();//Check if they should roll again
                        }
                        else if (moveOnBoardToken.IsChecked == true)
                        {
                            //Enable selection for moving a token on the board 6 steps
                            EnableTokenSelectionForSixSteps(currentPlayerIndex);
                            return;//Wait for token selection
                        }
                    }
                }
                else
                {
                    //If only pieces on board, allow moving one of them 6 steps
                    EnableTokenSelectionForSixSteps(currentPlayerIndex);
                    return;//Wait for token selection, don't pass the turn yet
                }
            }
            else if (diceRoll == 1)
            {
                //Handle roll of 1 in a similar fashion
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
                            AnimateToken(currentPlayerIndex, 0, tokenToMoveOut);

                            DisableTokenSelection();//Disable further token movement
                            PassTurnOrEnableRollForSix();//Check if they should roll again or pass
                        }
                        else if (moveOnBoardToken.IsChecked == true)
                        {
                            EnableTokenSelectionForOneStep(currentPlayerIndex);//Allow moving a token on the board by 1 step
                            return;//Wait for token selection, don't pass the turn yet
                        }
                    }
                }
                else if (hasPiecesInNest && !hasPiecesOnBoard)
                {
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                    AnimateToken(currentPlayerIndex, 0, tokenToMoveOut);

                    DisableTokenSelection();//Disable further token movement
                    PassTurnOrEnableRollForSix();//Check if they should roll again or pass
                }
                else if (!hasPiecesInNest && hasPiecesOnBoard)
                {
                    EnableTokenSelectionForOneStep(currentPlayerIndex);//Allow moving a token on the board by 1 step
                    return;//Wait for token selection, don't pass the turn yet
                }
            }
            else
            {
                EnableTokenSelection(currentPlayerIndex);//Enable selecting a token to move after rolling
            }

            if (players[currentPlayerIndex].Type == Player.PlayerType.Computer)
            {
                await HandleComputerTurn();
                return;
            }
        }

        /// <summary>
        /// Checks the last rolled dice value and either allows the current player to roll again if it was a 6, 
        /// or passes the turn to the next player if it was not.
        /// </summary>
        private void PassTurnOrEnableRollForSix()
        {
            if (diceRoll == 6)
            {
                //Allow player to roll again after rolling a 6
                DiceRollResult.Text += " (Player gets to roll again!)";
                EnableDiceForCurrentPlayer();//Allow rolling again
            }
            else
            {
                diceRoll = 0;  //Reset dice roll if not 6
                PassTurnToNextPlayer(); //Pass turn to the next player
            }
        }

        /// <summary>
        /// Enables the token selection for the specified player based on the current game state.
        /// Tokens can only be selected if they are either on the board or in the nest when a 1 or 6 is rolled.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose tokens are being enabled for selection.</param>
        private void EnableTokenSelection(int playerIndex)
        {
            //Make the current player's tokens selectable
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                //Allow selection of tokens in the nest (-1) ONLY if a 1 or 6 is rolled, otherwise skip nest tokens
                if (tokenPosition >= 0 || (tokenPosition == -1 && (diceRoll == 1 || diceRoll == 6)))
                {
                    token.IsTapEnabled = true;  //Make the token tappable if valid
                }
                else
                {
                    token.IsTapEnabled = false; //Ensure nest tokens are not tappable unless 1 or 6 is rolled
                }
            }

            //Prompt the player to choose a token
            DiceRollResult.Text = $"Select a token to move.";
        }

        /// <summary>
        /// Enables the token selection for the specified player to allow moving a token 6 steps.
        /// Only tokens that are currently on the board can be selected for this move.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose tokens are being enabled for selection.</param>
        private void EnableTokenSelectionForSixSteps(int playerIndex)
        {
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                //Only enable pieces already on the board (position >= 0) for moving 6 steps
                if (tokenPosition >= 0 && tokenPosition < 99)
                {
                    token.IsTapEnabled = true;
                }
            }

            //Prompt the player to choose a token to move 6 steps
            DiceRollResult.Text = $"Select a token on the board to move 6 steps.";
        }

        /// <summary>
        /// Enables the token selection for the specified player to allow moving a token 1 step.
        /// Only tokens that are currently on the board can be selected for this move.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose tokens are being enabled for selection.</param>
        private void EnableTokenSelectionForOneStep(int playerIndex)
        {
            for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
            {
                Grid token = GetPlayerToken(playerIndex, tokenIndex);
                int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                //Only enable pieces already on the board (position >= 0) for moving 1 step
                if (tokenPosition >= 0 && tokenPosition < 99)
                {
                    token.IsTapEnabled = true;
                }
                else
                {
                    token.IsTapEnabled = false; //Ensure that tokens in the nest aren't selectable
                }
            }

            //Prompt the player to choose a token to move 1 step
            DiceRollResult.Text = $"Select a token on the board to move 1 step.";
        }

        /// <summary>
        /// Called when the page becomes active and is about to be displayed to the user.
        /// Initializes the players based on the types received from the game settings
        /// and sets the current player to a random player, ensuring they are of type Player.
        /// </summary>
        /// <param name="e">The event data containing navigation parameters.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			//Ensure we received player types from the game settings
			if (e.Parameter is List<Player.PlayerType> playerTypes)
			{
				//Call the overloaded constructor to initialize players with the correct types
				players = new Player[]
				{
			        new Player("red") { Type = playerTypes[0] },
			        new Player("blue") { Type = playerTypes[1] },
			        new Player("green") { Type = playerTypes[2] },
			        new Player("yellow") { Type = playerTypes[3] }
				};

                //Set a random player as starting
                currentPlayerIndex = random.Next(0, players.Length);

                //If random player wasnt of type Player, look for the first player of type Player
                if (players[currentPlayerIndex].Type != Player.PlayerType.Player)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].Type == Player.PlayerType.Player)
                        {
                            currentPlayerIndex = i;
                            break;
                        }
                    }
                }

                DiceIsEnable(currentPlayerIndex);
			}
			for (int i = 0; i < players.Length; i++)
			{
				Debug.WriteLine($"MainPage - Player {i + 1} type: {players[i].Type}");
			}
		}

        /// <summary>
        /// Highlights the selected token by applying a new stroke thickness and adding a drop shadow.
        /// This method is designed to modify the visual appearance of a token when it is tapped,
        /// making it clear to the player which token is currently selected.
        /// </summary>
        /// <param name="tokenGrid">The Grid element representing the token that is selected.</param>
        private void HighlightSelectedToken(Grid tokenGrid)
        {
            AddDropShadow(tokenGrid);

            foreach (var child in tokenGrid.Children)//Adding new strokethickness to the tokens
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

        /// <summary>
        /// Adds a drop shadow effect to the specified target element (Grid).
        /// The shadow enhances the visual appearance of the element, making it stand out.
        /// </summary>
        /// <param name="targetElement">The Grid element to which the drop shadow will be applied.</param>
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

        /// <summary>
        /// Resets visual effects on the specified token element (Grid).
        /// This method removes any applied shadow effects and resets the stroke thickness of the token's visual representation.
        /// </summary>
        /// <param name="tokenGrid">The Grid element representing the token whose effects will be reset.</param>
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

        /// <summary>
        /// Handles the selection of a chosen token by adding tap event handlers to each player's token.
        /// This method is called when a player needs to select a token for movement.
        /// </summary>
        /// <param name="sender">The source of the event, typically the UI element that was tapped.</param>
        /// <param name="e">Event data containing the details of the tap event.</param>
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

        /// <summary>
        /// Handles the event when a player taps on a token. This method checks if the tapped token belongs to the 
        /// current player and processes the token movement based on the game's rules.
        /// </summary>
        /// <param name="sender">The source of the event, typically the token that was tapped.</param>
        /// <param name="e">Event data containing the details of the tap event.</param>
        private void OnTokenTapped(object sender, TappedRoutedEventArgs e)
        {
            Grid clickedToken = sender as Grid;
            //First deselect all tokens
            DeselectAllTokens();

            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);

                    if (token == clickedToken && playerIndex == currentPlayerIndex)
                    {
                        int tokenPosition = players[playerIndex].GetTokenPosition(tokenIndex);

                        //If the token is in the nest and the player rolled a 1 or 6, move it out of the nest
                        if (tokenPosition == -1 && (diceRoll == 1 || diceRoll == 6))
                        {
                            players[currentPlayerIndex].MoveOutOfNest(tokenIndex);
                            AnimateToken(currentPlayerIndex, 0, tokenIndex);  //Move to the start position
                            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} moved a token out of the nest!";
                        }
                        //If the token is on the board, move it based on the dice roll
                        else if (tokenPosition >= 0 && tokenPosition != 99)
                        {
                            AnimateToken(currentPlayerIndex, diceRoll, tokenIndex);
                            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} moved a token!";
                        }

                        //Disable token selection after a move
                        selectedTokenIndex = -1;
                        DisableTokenSelection();

                        //Reset the dice roll value after a move
                        diceRoll = 0;

                        //If the player rolled a 6, allow reroll after moving
                        if (diceRoll == 6)
                        {
                            DiceRollResult.Text += " (Player gets to roll again!)";
                            EnableDiceForCurrentPlayer(); //Allow reroll
                        }
                        else
                        {
                            PassTurnToNextPlayer(); //Pass the turn if it's not a 6
                        }

                        return; //Exit after handling the token tap
                    }
                    HighlightSelectedToken(clickedToken); //Highlight the selected token
                }
            }
        }

        /// <summary>
        /// Deselects all tokens on the board by resetting their visual effects. 
        /// This method ensures that no tokens are highlighted after a move or when 
        /// the player needs to make a new selection.
        /// </summary>
        private void DeselectAllTokens()
        {
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);

                    //Reset visual effetcs (highlighting)
                    ResetTokenEffects(token);
                }
            }

            //Reset the selected token
            selectedTokenIndex = -1;
        }

        /// <summary>
        /// Passes the turn to the next player in the game. 
        /// This method increments the current player index, 
        /// resets the dice roll, and skips any players that are inactive 
        /// (of type None). If the next player is a computer, it automatically 
        /// handles their turn.
        /// </summary>
        private void PassTurnToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            diceRoll = 0;//Reset the dice roll here

            //If current player is none get next good player
            while (players[currentPlayerIndex].Type == Player.PlayerType.None)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
                Debug.WriteLine($"Skipped {players[currentPlayerIndex].Name} because type is None.");
            }

            if (players[currentPlayerIndex].Type == Player.PlayerType.Computer)
            {
                Debug.WriteLine("Next player is Computer, handling their turn.");
                _ = HandleComputerTurn();
            }

            DiceIsEnable(currentPlayerIndex);//Enable the next player's dice
        }

        /// <summary>
        /// Checks whether the turn should be passed to the next player based on the 
        /// current player's movement and dice roll. If the player has moved or if 
        /// all pieces are in the nest and the roll is not 1 or 6, it passes the turn. 
        /// Otherwise, it allows the current player to roll again if they rolled 1 or 6.
        /// </summary>
        /// <param name="hasMoved">Indicates whether the current player has moved any token.</param>
        private void PassTurnIfNeeded(bool hasMoved)
        {
            //Check if all pieces are in the nest
            bool allPiecesInNest = players[currentPlayerIndex].HasPiecesInNest();

            //If the player has moved, or all pieces are in the nest and they didn't roll 1 or 6, pass the turn
            if (hasMoved || (allPiecesInNest && (diceRoll != 1 && diceRoll != 6)))
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
                diceRoll = 0;  //Reset the dice roll here
                DiceIsEnable(currentPlayerIndex); //Enable the next player's dice
            }
            else
            {
                //If the player cannot move (all pieces in nest) and rolled 1 or 6, they get another chance
                DiceIsEnable(currentPlayerIndex);//Keep the current player's turn
            }
        }

        /// <summary>
        /// Disables the ability to tap on all tokens for all players. 
        /// This prevents any token movements until re-enabled.
        /// </summary>
        private void DisableTokenSelection()
        {
            //Disable tapping on tokens for all players
            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
            {
                for (int tokenIndex = 0; tokenIndex < 4; tokenIndex++)
                {
                    Grid token = GetPlayerToken(playerIndex, tokenIndex);
                    token.IsTapEnabled = false;
                }
            }
        }

        /// <summary>
        /// Disables the dice button for the current player, preventing them from rolling again.
        /// </summary>
        private void DisableDiceForCurrentPlayer()
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
            diceButtons[currentPlayerIndex].IsEnabled = false;
        }

        /// <summary>
        /// Enables the dice button for the current player, allowing them to roll the dice again.
        /// </summary>
        private void EnableDiceForCurrentPlayer()
        {
            Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
            diceButtons[currentPlayerIndex].IsEnabled = true;
        }

        /// <summary>
        /// Simulates rolling a six-sided dice by returning a random integer between 1 and 6.
        /// </summary>
        /// <returns>A random integer representing the outcome of the dice roll.</returns>
        private int RollDice()
        {
            return random.Next(1, 7);
        }

        /// <summary>
        /// Applies a fade-out animation to the specified UI element.
        /// </summary>
        /// <param name="targetElement">The UI element to fade out.</param>
        /// <param name="onCompleted">An optional action to execute when the animation completes.</param>
        private void ApplyFadeOutAnimation(UIElement targetElement, Action onCompleted = null)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            Storyboard fadeOutStoryboard = new Storyboard();
            fadeOutStoryboard.Children.Add(fadeOutAnimation);

            Storyboard.SetTarget(fadeOutAnimation, targetElement);
            Storyboard.SetTargetProperty(fadeOutAnimation, "Opacity");

            if (onCompleted != null)
            {
                fadeOutStoryboard.Completed += (s, e) => onCompleted();
            }

            fadeOutStoryboard.Begin();
        }

        /// <summary>
        /// Applies a fade-in animation to the specified UI element.
        /// </summary>
        /// <param name="targetElement">The UI element to fade in.</param>
        private void ApplyFadeInAnimation(UIElement targetElement)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            Storyboard fadeInStoryboard = new Storyboard();
            fadeInStoryboard.Children.Add(fadeInAnimation);

            Storyboard.SetTarget(fadeInAnimation, targetElement);
            Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");

            fadeInStoryboard.Begin();
        }

        /// <summary>
        /// Animates a player's token by fading it out and moving it a specified number of steps.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose token is being animated.</param>
        /// <param name="steps">The number of steps to move the token.</param>
        /// <param name="tokenIndex">The index of the token to animate.</param>
        private void AnimateToken(int playerIndex, int steps, int tokenIndex)
        {
            //Get the correct token to animate
            Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);

            //Fade out selected token before moving it
            ApplyFadeOutAnimation(playerToken, () => MovePlayer(playerIndex, steps, tokenIndex, playerToken));
        }//The separation of AnimateToken and MovePlayer provides a more accurate fade out and fade in effect

        /// <summary>
        /// Moves the specified player's token a given number of steps on the board.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose token is being moved.</param>
        /// <param name="steps">The number of steps to move the token.</param>
        /// <param name="tokenIndex">The index of the token to move.</param>
        /// <param name="playerToken">The visual representation of the token.</param>
        private void MovePlayer(int playerIndex, int steps, int tokenIndex, Grid playerToken)
        {
            int currentPosition = players[playerIndex].GetTokenPosition(tokenIndex);

            //If the piece is already in goal, do nothing
            if (currentPosition == 99)
            {
                return;
            }

            //Move the piece from the nest to the start if it is still in the nest
            if (currentPosition == -1)
            {
                players[playerIndex].SetTokenPosition(tokenIndex, 0);
                players[playerIndex].PiecesInNest--;
            }
            else if (currentPosition + steps > GetPlayerPath(playerIndex).Length - 1)
            {
                //If the throw causes the piece to go past the target, move back the excess steps
                var path = GetPlayerPath(playerIndex);
                int moveBack = PacesToMoveBack(currentPosition, path.Length, steps);
                int newPositionOnBoard = currentPosition - moveBack;
                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);
                playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
            }
            else
            {
                //Play Motion Audio
                SoundManager.PlaySound(SoundType.PieceMove);

                //Move the token forward with the number of steps
                int newPositionOnBoard = currentPosition + steps;
                var path = GetPlayerPath(playerIndex);

                if (newPositionOnBoard >= path.Length)
                {
                    newPositionOnBoard = path.Length - 1;
                }

                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);

                //Check if the token has reached the goal
                if (newPositionOnBoard == path.Length - 1)
                {
                    players[playerIndex].SetTokenPosition(tokenIndex, 99);//Mark that the piece is in goal
                    HandlePlayerGoal(playerIndex, tokenIndex);//Check if the player has won
                }

                //Move the token visually
                playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
                CheckForOverlappingTokens(playerIndex, tokenIndex);

                ResetTokenEffects(playerToken);
            }

            ApplyFadeInAnimation(playerToken);
        }

        /// <summary>
        /// Checks for overlapping tokens after a player moves a token.
        /// If an overlapping token is found, it is sent back to its nest.
        /// </summary>
        /// <param name="playerIndex">The index of the player who moved the token.</param>
        /// <param name="tokenIndex">The index of the token that was moved.</param>
        private void CheckForOverlappingTokens(int playerIndex, int tokenIndex)
        {
            //Get the Grid.Row and Grid.Column of the moved token
            Grid movedTokenGrid = GetPlayerToken(playerIndex, tokenIndex);
            int movedTokenRow = Grid.GetRow(movedTokenGrid);
            int movedTokenCol = Grid.GetColumn(movedTokenGrid);

            //Loop through all other players to check if any token is on the same grid location
            for (int otherPlayerIndex = 0; otherPlayerIndex < players.Length; otherPlayerIndex++)
            {
                if (otherPlayerIndex == playerIndex) continue; // Skip checking the current player's own tokens

                //Loop through the tokens of the other player
                for (int otherTokenIndex = 0; otherTokenIndex < 4; otherTokenIndex++)
                {
                    Grid otherTokenGrid = GetPlayerToken(otherPlayerIndex, otherTokenIndex);
                    int otherTokenRow = Grid.GetRow(otherTokenGrid);
                    int otherTokenCol = Grid.GetColumn(otherTokenGrid);

                    //Compare the grid positions of both tokens
                    if (movedTokenRow == otherTokenRow && movedTokenCol == otherTokenCol)
                    {
                        //Exclude the goal position (5, 5) from knockouts
                        if (movedTokenRow == 5 && movedTokenCol == 5)
                        {
                            continue;//Skip knockout for tokens in the goal position
                        }

                        //Push the other player's token back to the nest
                        players[otherPlayerIndex].SetTokenPosition(otherTokenIndex, -1); // -1 means back to the nest
                        players[otherPlayerIndex].PiecesInNest++; // Increment the opponent's PiecesInNest count

                        //Remove the token from the board visually
                        ApplyFadeOutAnimation(otherTokenGrid);
                        RepopulateNest(otherPlayerIndex, otherTokenIndex);

                        //Optionally, display a message about the knockout
                        DiceRollResult.Text = $"{IndexToName(playerIndex)} knocked out {IndexToName(otherPlayerIndex)}'s piece!";

                        //Break out after knocking out one piece
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Repopulates the specified token back to the player's nest position.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose token is being repopulated.</param>
        /// <param name="tokenIndex">The index of the token to repopulate.</param>
        private void RepopulateNest(int playerIndex, int tokenIndex)
        {
            //Get the player token visually
            Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);

            //Use the existing nest coordinates from the player grid positions
            switch (playerIndex)
            {
                case 0://Red player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 0, 0);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 0, 1);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 1, 0);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 1, 1);
                    ApplyFadeInAnimation(playerToken);
                    break;

                case 1://Blue player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 0, 9);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 0, 10);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 1, 9);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 1, 10);
                    ApplyFadeInAnimation(playerToken);
                    break;

                case 2://Green player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 9, 9);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 9, 10);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 10, 9);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 10, 10);
                    ApplyFadeInAnimation(playerToken);
                    break;

                case 3://Yellow player
                    if (tokenIndex == 0) SetTokenPosition(playerToken, 9, 0);
                    if (tokenIndex == 1) SetTokenPosition(playerToken, 9, 1);
                    if (tokenIndex == 2) SetTokenPosition(playerToken, 10, 0);
                    if (tokenIndex == 3) SetTokenPosition(playerToken, 10, 1);
                    ApplyFadeInAnimation(playerToken);
                    break;
            }
        }

        /// <summary>
        /// Finds the index of the next token in the nest for the specified player.
        /// </summary>
        /// <param name="playerIndex">The index of the player to check.</param>
        /// <returns>The index of the next token in the nest, or -1 if all tokens are out.</returns>
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

        /// <summary>
        /// Finds the index of the next token on the board for the specified player.
        /// </summary>
        /// <param name="playerIndex">The index of the player to check.</param>
        /// <returns>The index of the next token on the board, or -1 if no tokens are available.</returns>
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

        /// <summary>
        /// Calculates the number of paces a player needs to move back if they overshoot the goal.
        /// </summary>
        /// <param name="position">The current position of the player token.</param>
        /// <param name="pathLength">The total length of the path.</param>
        /// <param name="steps">The number of steps rolled on the dice.</param>
        /// <returns>The number of paces to move back, if the token overshoots the goal; otherwise, returns 0.</returns>
        private int PacesToMoveBack(int position, int pathLength, int steps)
		{
			int pacesToGoal = (pathLength - 1) - position;
			int moveBackPaces = (position + steps) - (pathLength - 1);
			return moveBackPaces - pacesToGoal; //Returns the amount of paces to go back if larger than paces to goal
		}

        /// <summary>
        /// Handles the logic when a player reaches the goal with one of their tokens.
        /// </summary>
        /// <param name="playerIndex">The index of the player who has reached the goal.</param>
        /// <param name="tokenIndex">The index of the token that has reached the goal.</param>
        private void HandlePlayerGoal(int playerIndex, int tokenIndex)
        {
            //Get the color name of the player
            string playerColor = IndexToName(playerIndex);

            //Update the result text to show that the player has reached the goal
            DiceRollResult.Text = $"Player {playerColor} has reached the goal with one of their pieces!";

            //Mark the piece as in goal by setting the position to 99
            players[playerIndex].SetTokenPosition(tokenIndex, 99);

            //Check if all the pieces for the player are in goal
            if (players[playerIndex].AllPiecesInGoal())
            {
                //If all pieces are in goal, mark that the player has won
                players[playerIndex].HasWon = true;

                //Update the result text to show that the player has won the game
                DiceRollResult.Text += $" {playerColor} has won the game!";
                //Play the win sound
                SoundManager.PlaySound(SoundType.Win);

                //Here you can add logic to end the game or display a pop-up message about the win
                //For example, you can add a ContentDialog to let you know that the game is over
                var winDialog = new ContentDialog
                {
                    Title = "Game Over",
                    Content = $"Congratulations! {playerColor} has won the game!",
                    CloseButtonText = "OK"
                };

                //Show win dialog
                _ = winDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Converts a player index to the corresponding color name.
        /// </summary>
        /// <param name="index">The index of the player (0 for Red, 1 for Blue, 2 for Green, 3 for Yellow).</param>
        /// <returns>The color name as a string, or null if the index is out of range.</returns>
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

        /// <summary>
        /// Retrieves the specified token for a given player index and token index.
        /// </summary>
        /// <param name="playerIndex">The index of the player (0 for Player 1, 1 for Player 2, 2 for Player 3, 3 for Player 4).</param>
        /// <param name="tokenIndex">The index of the token (0 to 3).</param>
        /// <returns>The <see cref="Grid"/> representing the player's token, or null if the indices are out of range.</returns>
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

        /// <summary>
        /// Retrieves the path coordinates for a given player based on the player index.
        /// </summary>
        /// <param name="playerIndex">The index of the player (0 for Red, 1 for Blue, 2 for Green, 3 for Yellow).</param>
        /// <returns>An array of tuples representing the path coordinates for the specified player, or null if the player index is invalid.</returns>
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

        /// <summary>
        /// Sets the position of a token within a grid by specifying its row and column indices.
        /// </summary>
        /// <param name="token">The token (Grid element) to position within the grid.</param>
        /// <param name="row">The row index where the token should be placed.</param>
        /// <param name="col">The column index where the token should be placed.</param>
        private void SetTokenPosition(Grid token, int row, int col)
        {
            Grid.SetRow(token, row);
            Grid.SetColumn(token, col);
        }

        /// <summary>
        /// Navigates back to the main menu when triggered by a UI event.
        /// </summary>
        /// <param name="sender">The source of the event, typically the UI element that triggered the action.</param>
        /// <param name="e">The routed event arguments containing event data.</param>
        private void Back_to_MainMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));

            //Kill game incase 4 AI's selected, otherwise task will be in background
            stopGame = true;
        }
    }
}
