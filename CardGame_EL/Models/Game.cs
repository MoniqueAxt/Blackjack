using CardGame_EL.Enums;
using CardGame_EL.GameEvents;
using CardGame_EL.Players;
using System.ComponentModel;

namespace CardGame_EL.Models;
/// <summary>
/// Represents a Card game.
/// </summary>
public class Game
{
    public int Id { get; set; }

    public string GameName { get; set; } = "GameNameDefault";

    public virtual BindingList<Player> Players { get; set; }    /* virtual keyword used to be able to 'automagically' load game properties when getting data from db using Dbcontext */

    public virtual Player ActivePlayer { get; set; }

    public virtual Player Dealer { get; set; }

    private IEnumerator<GameEvent>? _processEnumerator;
    //public IEnumerator<GameEvent>? ProcessEnumerator { get; internal set; }

    public List<GameEvent> ProcessGameEvents
    {
        get
        {
            if (_processEnumerator == null) return new List<GameEvent>();

            List<GameEvent> gameEvents = new();
            while (_processEnumerator.MoveNext())
            {
                gameEvents.Add(_processEnumerator.Current);
            }
            return gameEvents;

        }
        internal set
        {
            //ProcessEnumerator = value;     
            var events = value as IEnumerable<GameEvent>;
            _processEnumerator = events.GetEnumerator();
        }
    }

    public virtual Deck GameDeck { get; set; }

    public bool GameOver { get; set; }

    public GameSettings GameSettings { get; set; }


    /**********************************************************************
     * Constructors
     /**********************************************************************/
    public Game()
    {
    }

    /// <summary>
    /// Default constructor with default values:
    ///     * Dealer and Players are created
    ///     * One (1) game deck is created and shuffled     
    /// </summary>
    public Game(GameSettings settings)
    {
        GameSettings = settings;

        Players = new BindingList<Player>();
        Dealer = new BlackJackPlayer("Dealer", false);
        Players.Add(Dealer);

        // create players based on number of players
        int nrHumans = GameSettings.NumberHumanPlayers;
        for (int i = 1; i <= GameSettings.NumberPlayersToCreate; i++, nrHumans--)
        {
            Players.Add(new BlackJackPlayer("Player " + i, nrHumans > 0));
        }

        ActivePlayer = Dealer;

        GameDeck = new Deck();
        GameDeck.LoadNewDeck(GameSettings.NumberDecks);
        GameDeck.Shuffle();

    }

    // Given: players and the dealer are specified
    public Game(BindingList<Player> players, Player dealer, GameSettings settings)
    {
        GameSettings = settings;
        Players = players;
        Dealer = dealer;
        ActivePlayer = dealer;

        GameDeck = new Deck();
        GameDeck.LoadNewDeck(GameSettings.NumberDecks);
        GameDeck.Shuffle();
    }

    // Given: players, the deck of cards, and the dealer is specified
    public Game(BindingList<Player> players, Player dealer, GameSettings settings, Deck gameDeck)
     : this(players, dealer, settings)
    {
        GameDeck = gameDeck;
    }

    /**********************************************************************
    * Methods
    /**********************************************************************/

    /// <summary>
    /// Sets player status "HasLost" to true if the player has bust (> 21 Score)
    /// </summary>
    public virtual void UpdateGameState()
    {
        if (ActivePlayer.Score > 21) ActivePlayer.Status = Status.Bust;

        // game is not over
        if (Players.Any(player => player.Status == Status.Playing)) return;

        // game is over
        GameOver = true;
        HandleGameOver();
    }

    /// <summary>
    /// Updates game state once the game is over. Determines who won.
    /// </summary>
    public void HandleGameOver()
    {
        List<Player> nonBustPlayers = new();
        nonBustPlayers.AddRange(Players.Where(p => p.Status != Status.Bust && p != Dealer));

        // The Dealer went bust, so all non-bust players are winners
        if (Dealer.Status == Status.Bust)
        {
            nonBustPlayers.ForEach(p => p.Status = Status.Won);
        }

        // The Dealer didn't go bust
        else
        {
            // check which players beat the Dealer
            foreach (Player p in nonBustPlayers.Where(p => p.Score > Dealer.Score))
            {
                p.Status = Status.Won;
            }

            // check if the Dealer won (if any of the non-bust players have NOT won, by default the Dealer has won against that player)
            if (nonBustPlayers.Any(p => p.Status != Status.Won))
            {
                Dealer.Status = Status.Won;
            }
        }
    }

    /// <summary>
    /// Add a player to the game
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    /** ********************************************************************
     * Deck methods
     /**********************************************************************/

    /// <summary>
    /// Deal a given number of cards to each player.
    /// Returns an Enumerable collection of NOT executed actions
    /// </summary>
    /// <param name="nrCards"></param>
    /// <returns></returns>
    public IEnumerable<GameEvent> DealInitialCards(int nrCards)
    {
        Queue<GameEvent> actions = new();

        // deal cards, one card per round to each player
        for (int i = 0; i < nrCards; i++)
        {
            foreach (Player player in Players)
            {
                // dealer only gets 1 card regardless of how many other players get
                if (player == Dealer && i > 0)
                    continue;

                ReceiveCardEvent gameEvent = new ReceiveCardEvent(GameDeck, player.Hand, player);
                actions.Enqueue(gameEvent);
            }
        }

        foreach (GameEvent action in actions)
        {
            yield return action;
        }
    }


    /// <summary>
    /// Creates a stream of actions that players made and returns the next action in the stream.
    /// </summary>
    /// <returns></returns>
    public GameEvent Process()
    {
        // create the enumerator
        if (_processEnumerator == null)
        {
            _processEnumerator = CreateProcessEnumerable().GetEnumerator();
        }

        // advance one step; if no more steps - game over
        if (!_processEnumerator.MoveNext())
        {
            return new GameOverEvent();
        }

        var current = _processEnumerator.Current;
        return current;
    }

    /// <summary>
    /// Creates an enumerable of actions selected by players
    /// </summary>
    /// <returns></returns>
    private IEnumerable<GameEvent> CreateProcessEnumerable()
    {
        while (!GameOver)
        {
            foreach (Player player in Players)
            {
                ActivePlayer = player;

                if (ActivePlayer == Dealer)
                {
                    // if any of the non-dealer players are still Playing, it is not the Dealer's turn
                    bool playersDone = !Players.Any(p => p != Dealer && p.Status == Status.Playing);
                    if (!playersDone)
                        continue;
                }

                while (player.Status == Status.Playing)    /* one player dealt with at a time */
                {
                    if (player.IsHuman)
                    {
                        yield return new UserInputRequired(player);
                    }
                    else
                    {
                        GameEvent gameEvent = player.GetAction(this);
                        yield return gameEvent;
                    }
                }
            }
        }

        if (GameOver)
            yield return new GameOverEvent();
    }


}