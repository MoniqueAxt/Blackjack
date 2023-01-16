using BlackJack_PL;
using CardGame_BLL;
using CardGame_EL.Enums;
using CardGame_EL.GameEvents;
using CardGame_EL.Models;
using CardGame_EL.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Utilities;
using Image = System.Windows.Controls.Image;

namespace BlackJack_CardGame;

public delegate void NotifyStartEvent(GameSettings settings);
public delegate void NotifySelectionEvent(Game selected);

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Game _game = null!;
    private DatabaseManager _gameManager;
    private readonly FileLogger _fileOutput;
    private IEnumerator<GameEvent> _actionEnumerator = null!;
    private readonly NotifyStartEvent _notifyInitEvent;
    private readonly NotifySelectionEvent _notifySelectionEvent;
    private readonly Timer _timer;

    public MainWindow()
    {
        InitializeComponent();
        _gameManager = new DatabaseManager();

        _fileOutput = new FileLogger("blackjack_log.txt");
        Logger.WriteMessage += LoggingMethods.LogToConsole;
        _notifyInitEvent += InitNewGame;
        _notifySelectionEvent += LoadGame;

        _timer = new Timer();
        _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        _timer.AutoReset = false;   //start the animation immediately
        _timer.Enabled = true;      // start the timer
        ToggleButtons(false);
    }

    //TODO
    private void LoadGame(Game selected)
    {
        _game = selected;
    }

    //TODO
    /// <summary> 
    /// Load a previously saved game from the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MnuLoad_Click(object sender, RoutedEventArgs e)
    {
        // List<Game> dbGames = _gameManager.LoadDbGames();       
        // new Window2(dbGames, _notifySelectionEvent).ShowDialog();
        // this.Show();       
    }

    /// <summary>
    /// Save a game to the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MnuSave_Click(object sender, RoutedEventArgs e)
    {
        if (_gameManager.GameInstance != null)
        {
            if (!_gameManager.GameInstance.GameOver)
            {
                MessageBox.Show("Finish the game before saving", "Error");
                return;
            }

            int insertedId = _gameManager.SaveGameInstanceToDb();
            MessageBox.Show("Game saved! " + $" [{insertedId}]");
        }
        else
        {
            MessageBox.Show("No game to save", "Error");
        }

    }

    private void MnuClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// Set up the conditions for a new game state. 
    /// </summary>
    private void InitNewGame(GameSettings settings)
    {
        _game = new Game(settings);
        _gameManager = new DatabaseManager(_game);
        //_gameManager.GameInstance = _game;
        Logger.LogMessage($"New Game! [{_game.Players.Count} players, {_game.GameSettings.NumberDecks} decks]");

        // bind Listbox displaying the list of players
        lbxPlayers.ItemsSource = null;
        lbxPlayers.ItemsSource = _game.Players;

        // update labels
        LblNrDecks.Content = _game.GameSettings.NumberDecks;
        LblNrPlayers.Content = _game.Players.Count - 1; // exclude the dealer

        ResetCardsVisual();

        // deal 2 cards to each player at the start of the game
        _actionEnumerator = _game.DealInitialCards(2).GetEnumerator();

        while (_actionEnumerator.MoveNext())
        {
            GameEvent gameEvent = _actionEnumerator.Current;
            gameEvent.Execute();
            LogEvent(gameEvent);
            RenderUI(gameEvent);
        }

        GameLoop();
    }
    /// <summary>
    /// The main game-loop of the game.
    /// Processes events received from the game, executes actions based on the events, then updates the game-state and UI.
    /// </summary>
    private async void GameLoop()
    {
        GameEvent gameEvent = new NoAction(null!);

        while (gameEvent is not UserInputRequired && gameEvent is not GameOverEvent)
        {
            gameEvent = _game.Process();
            gameEvent.Execute();
            LogEvent(gameEvent);

            _game.UpdateGameState();

            RenderUI(gameEvent);

            await Task.Delay(700);    //Slow down the game to imitate turn-taking
        }

        // handle game over
        if (gameEvent is GameOverEvent)
        {
            HandleGameOver(gameEvent);
        }
    }

    #region UI methods
    //=================================================================================
    // UI methods
    //=================================================================================
    /// <summary>
    /// 
    /// </summary>
    private void RenderUI(GameEvent gameEvent)
    {
        // inform the current (human) player that they've gone bust
        if (gameEvent is DrawCardEvent &&
            _game.ActivePlayer.IsHuman &&
            _game.ActivePlayer.Status == Status.Bust)
        {
            MessageBox.Show("Bust!");
        }

        // bind Listbox displaying other players to keep updated
        lbxPlayers.ItemsSource = null;
        lbxPlayers.ItemsSource = _game.Players;

        //update listbox / listview selected item
        lbxPlayers.SelectedItem = _game.ActivePlayer;

        //update labels
        if (_game.ActivePlayer == _game.Dealer)
        {
            LblDealerScore.Content = _game.ActivePlayer.Hand.Score;
        }
        else
        {
            lblActivePlayer.Content = _game.ActivePlayer.Name;
            lblScore.Content = _game.ActivePlayer.Hand.Score;
        }

        // set the listbox's datasources (representing the cards of the dealer and active player)
        lbxDealerHand.ItemsSource = null;
        lbxDealerHand.ItemsSource = _game.Dealer.Hand.Cards;
        lbxActivePlayerHand.ItemsSource = null;
        lbxActivePlayerHand.ItemsSource = _game.ActivePlayer.Hand.Cards;

        ToggleButtons(_game.ActivePlayer.IsHuman);

        UpdateCardsVisual(gameEvent);

    }

    /// <summary>
    /// Display the cards face-down
    /// </summary>
    private void ResetCardsVisual()
    {
        foreach (Image image in canPlayerCards.Children)
        {
            image.Source = new BitmapImage(new Uri(@"/Images/Card back.png", UriKind.Relative));
        }

        foreach (Image image in canDealerCards.Children)
        {
            image.Source = new BitmapImage(new Uri(@"/Images/Card back.png", UriKind.Relative));
        }
    }

    private void UpdateCardsVisual(GameEvent gameEvent)
    {
        // make the cards face-up when a player has a new card
        if (gameEvent is PlayerAction playerAction and not UserInputRequired)
        {
            int nrCardsInHand = playerAction.Owner.Hand.Cards.Count;
            int nrCardImages = canPlayerCards.Children.Count;

            for (int i = 0; i < nrCardsInHand && i < nrCardImages; i++)
            {
                Card card = playerAction.Owner.Hand.Cards.ElementAt(i);

                string imgName = "Images/" + card.Value + " of " + card.Suit + ".png";
                Uri uri = new Uri(imgName, UriKind.Relative);

                if (playerAction.Owner == _game.Dealer)
                {
                    ((Image)canDealerCards.Children[i]).Source = new BitmapImage(uri);
                }
                else
                {
                    ((Image)canPlayerCards.Children[i]).Source = new BitmapImage(uri);
                }
            }

            // Place the cards face-down if they're not being used/shown
            for (int i = nrCardsInHand; i < canDealerCards.Children.Count; i++)
            {
                Uri uri = new("Images/Card back.png", UriKind.Relative);

                ((Image)canDealerCards.Children[i]).Source = new BitmapImage(uri);
                ((Image)canPlayerCards.Children[i]).Source = new BitmapImage(uri);
            }
        }
    }

    /// <summary>
    /// Creates a gradient animation set to an instance of a timer
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        this.Dispatcher.Invoke(() =>
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.DodgerBlue);
            LblTitle.Background = brush;

            ColorAnimation animation = new ColorAnimation(Colors.DodgerBlue, Colors.MediumSeaGreen,
                new Duration(TimeSpan.FromSeconds(5)));

            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            _timer.Interval = 10000;    //re-set the timer interval
            _timer.Start();
        });
    }
    #endregion

    #region Helper Methods
    //=================================================================================
    // Helpers
    //=================================================================================
    /// <summary>
    /// Display the winners in the UI and log them to file.
    /// </summary>
    /// <param name="gameEvent"></param>
    private void HandleGameOver(GameEvent gameEvent)
    {
        if (gameEvent is not GameOverEvent) return;

        StringBuilder sb = new StringBuilder();
        foreach (Player gamePlayer in _game.Players)
        {
            if (gamePlayer.Status == Status.Won)
            {
                sb.Append(gamePlayer.Name).Append("\n");
            }
        }
        Logger.LogMessage(gameEvent + "\nWinner/s:\n" + sb);
        MessageBox.Show("Won:\n" + sb, "Game Over!");
    }

    /// <summary>
    /// Log each game event that takes place
    /// </summary>
    /// <param name="gameEvent"></param>
    private static void LogEvent(GameEvent gameEvent)
    {
        if (gameEvent is UserInputRequired) return; // don't log this event

        switch (gameEvent)
        {
            case PlayerAction pAction:
                Logger.LogMessage(gameEvent + ", " + pAction.Owner.Hand.Score);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Execute the action selected by the user, update the game state and restart the game loop
    /// </summary>
    /// <param name="actionToTake"></param>
    private void HandleUserInput(PlayerAction actionToTake)
    {
        actionToTake.Execute();
        LogEvent(actionToTake);
        _game.UpdateGameState();
        RenderUI(actionToTake);
        GameLoop();
    }

    /// <summary>
    /// Toggle buttons to be enabled or disabled depending on the passed parameter
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleButtons(bool enable)
    {
        BtnHit.IsEnabled = enable;
        BtnStand.IsEnabled = enable;
        BtnShuffle.IsEnabled = enable;
    }
    #endregion

    //=================================================================================
    // Button methods
    //=================================================================================
    private void BtnNewGame_Click(object sender, RoutedEventArgs e)
    {
        new Window1(_notifyInitEvent).ShowDialog();
        this.Show();
    }

    /// <summary>
    /// The player chooses to "hit" - draw a card
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnHit_Click(object sender, RoutedEventArgs e)
    {
        PlayerAction action = new DrawCardEvent(_game.GameDeck, _game.ActivePlayer.Hand, _game.ActivePlayer);
        _game.ActivePlayer.Status = Status.Playing;
        HandleUserInput(action);
    }

    /// <summary>
    /// The player chooses to "stand" - no further action is taken
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnStand_Click(object sender, RoutedEventArgs e)
    {
        PlayerAction action = new NoAction(_game.ActivePlayer);
        _game.ActivePlayer.Status = Status.Standing;
        HandleUserInput(action);
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Create a new game with default values
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnNewGameDefault_Click(object sender, RoutedEventArgs e)
    {
        InitNewGame(new GameSettings(numberPlayersToCreate: 5, numberHumanPlayers: 1));
    }

    /// <summary>
    /// Shuffle the cards of the deck 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnShuffle_Click(object sender, RoutedEventArgs e)
    {
        StringBuilder preShuffle = new StringBuilder();
        List<Card> firstFiveCards = ((List<Card>)_game.GameDeck.Cards).GetRange(0, 5);
        foreach (Card card in firstFiveCards) preShuffle.Append(card).Append("\n");
        firstFiveCards.Clear();

        _game.GameDeck.Shuffle();

        StringBuilder postShuffle = new StringBuilder();
        firstFiveCards = ((List<Card>)_game.GameDeck.Cards).GetRange(0, 5);
        foreach (Card card in firstFiveCards) postShuffle.Append(card).Append("\n");
        MessageBox.Show("Pre-shuffle:\n" + preShuffle.ToString() + "\n\nPost-shuffle:\n" + postShuffle);
    }

    /// <summary>
    /// Opens a new window displaying games that are saved in the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnBrowseDb_Click(object sender, RoutedEventArgs e)
    {
        List<Game> savedGames = DatabaseManager.GetAllGamesFromDb();
        new Window2(savedGames, _gameManager).ShowDialog();
        this.Show();
    }
}