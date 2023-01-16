using CardGame_DAL.Data;
using CardGame_EL.Models;
using System.Transactions;

namespace CardGame_BLL
{
    // (previously GameManager)
    public class DatabaseManager
    {
        public Game GameInstance { get; set; } = default!;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        public DatabaseManager(Game gameInstance)
        {
            GameInstance = gameInstance;
        }

        /// <summary>
        /// Constructor without parameters
        /// </summary>
        public DatabaseManager()
        { }

        /// <summary>
        /// Save the Game instance to the database
        /// </summary>
        /// <returns></returns>
        public int SaveGameInstanceToDb()
        {
            if (GameInstance == null)
                return -1;

            using GameDbContext context = new GameDbContext();

            // use Transaction Scope to avoid insertion issues due to the
            // circular reference between Game's Players and ActivePlayer / Dealer
            using (var ts = new TransactionScope())
            {
                context.AddRange(GameInstance.Players); // add Players first
                context.SaveChanges();

                context.Games.Add(GameInstance);
                ts.Complete();
            }

            return context.SaveChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Game> GetAllGamesFromDb()
        {
            using GameDbContext context = new();

            var games = from game in context.Games
                        select new Game()
                        {
                            Id = game.Id,
                            GameName = game.GameName,
                            Players = game.Players,
                            Dealer = game.Dealer,
                            GameSettings = game.GameSettings
                        };

            return games.ToList();
        }

        /// <summary>
        /// Delete a game from the database
        /// </summary>
        /// <param name="selectedGame"></param>
        public static void Delete(Game selectedGame)
        {
            using GameDbContext context = new();
            var dbGameToDelete = context.Games.Remove(selectedGame);

            context.SaveChanges();
        }

        /// <summary>
        /// Update a Game in the database
        /// </summary>
        /// <param name="selectedGame"></param>
        public static void Update(Game selectedGame)
        {
            using GameDbContext context = new();
            var dbGameToUpdate = context.Games.Update(selectedGame);

            context.SaveChanges();
        }

        /// <summary>
        /// Query the Db based on given parameters
        /// </summary>
        /// <param name="nrDecks"></param>
        /// <param name="nrPlayers"></param>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public List<Game> Filter(int nrDecks, int nrPlayers, string gameName)
        {
            // no filters given
            if (gameName.Equals("") && nrDecks < 0 && nrPlayers < 0)
            {
                return GetAllGamesFromDb().ToList();
            }

            using GameDbContext context = new();

            // ensure the initial lists are not empty so the intersect later works as intended
            var queryGameName = (from game in context.Games select game.Id).ToList();
            var queryDecks = queryGameName;
            var queryPlayers = queryGameName;

            // gameName filter was used : get the IDs of games that match the given gameName
            if (!gameName.Equals(""))
            {
                queryGameName = (from game in context.Games
                                 where (game.GameName).ToLower().Contains(gameName.ToLower())
                                 select game.Id).ToList();
            }

            // nrDecks filter was used : get the IDs of games that match the given number of decks
            if (nrDecks > 0)
            {
                queryDecks = (from game in context.Games
                              where game.GameSettings.NumberDecks == nrDecks
                              select game.Id).ToList();
            }

            // nrPlayers filter was used : get the IDs of games that match the given number of players
            if (nrPlayers > 0)
            {
                queryPlayers = (from game in context.Games
                                where game.Players.Count == nrPlayers
                                select game.Id).ToList();
            }

            // find the intersection of the 3 lists of IDs
            var intersect = queryGameName
                .Intersect(queryDecks)
                .Intersect(queryPlayers)
                .ToList();

            // query to fetch the corresponding games that match the intersecting IDs         
            var filteredGames =
                from game in context.Games
                where intersect.Contains(game.Id)
                select new Game()
                {
                    Id = game.Id,
                    GameName = game.GameName,
                    Dealer = game.Dealer,
                    Players = game.Players,
                    GameSettings = game.GameSettings
                };

            return filteredGames.ToList();
        }

    }
}
