using CardGame_EL.GameEvents;
using CardGame_EL.Models;
using CardGame_EL.Players;
using Microsoft.EntityFrameworkCore;

namespace CardGame_DAL.Data
{
    /// <summary>
    /// GameDb is the name of the database
    /// DbSet items represent tables
    /// </summary>
    public class GameDbContext : DbContext
    {

        public DbSet<Card> Cards { get; set; }

        public DbSet<Deck> Decks { get; set; }
        public DbSet<Hand> Hands { get; set; }


        public DbSet<Player> Players { get; set; }
        public DbSet<BlackJackPlayer> BlackJackPlayers { get; set; }


        public DbSet<Game> Games { get; set; }

        public DbSet<GameEvent> GameEvents { get; set; }
        public DbSet<GameStateAction> GameStateActions { get; set; }
        public DbSet<GameOverEvent> GameOverEvents { get; set; }


        public DbSet<PlayerAction> PlayerActions { get; set; }
        public DbSet<UserInputRequired> UserInputRequiredActions { get; set; }
        public DbSet<NoAction> NoActions { get; set; } = null!;
        public DbSet<DiscardCardEvent> DiscardCardEvents { get; set; }
        public DbSet<DrawCardEvent> DrawCardEvents { get; set; }
        public DbSet<ReceiveCardEvent> ReceiveCardEvents { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=GameDb");
            options.EnableSensitiveDataLogging();
        }
    }
}