using System.ComponentModel.DataAnnotations.Schema;

namespace CardGame_EL.Models
{
    /// <summary>
    /// Contains the common settings for a card game. Used to initialise a game instance quickly.
    /// </summary>
    public class GameSettings
    {        
        public int Id { get; set; }

        private int _numberDecks = 1;
        private int _numberRounds = 1;


        public int NumberDecks
        {
            get => _numberDecks;
            set => _numberDecks = AtLeastOne(value);
        }

        public int NumberRounds
        {
            get => _numberRounds;
            set => _numberRounds = AtLeastOne(value);
        }

        public int NumberPlayersToCreate { get; set; } = 0;

        public int NumberHumanPlayers { get; set; } = 0;

        [NotMapped]
        public int NumberBotPlayers { get; set; } = 0;
        //  public string FilePath { get; set; }

        public GameSettings() { }

        public GameSettings(int numberDecks = 1, int numberPlayersToCreate = 0, int numberRounds = 1, int numberHumanPlayers = 0, int numberBotPlayers = 0)
        {
            NumberDecks = numberDecks;
            NumberRounds = numberRounds;
            NumberPlayersToCreate = numberPlayersToCreate;
            NumberHumanPlayers = numberHumanPlayers;
            NumberBotPlayers = numberBotPlayers;
            // FilePath = filePath;
        }

        private static int AtLeastOne(int value)
        {
            return value < 1 ? 1 : value;
        }
    }
}
