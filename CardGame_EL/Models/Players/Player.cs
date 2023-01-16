using CardGame_EL.Enums;
using CardGame_EL.GameEvents;
using CardGame_EL.Models;

namespace CardGame_EL.Players
{
    public abstract class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Hand Hand { get; set; }

        public Status Status { get; set; }

        public bool IsHuman { get; set; }

        public int Score => Hand.Score;

        public bool HasLost => Status is Status.Bust;       /* used as Data Trigger in XAML*/

        public bool IsDone => Status is Status.Standing;    /* used as Data Trigger in XAML*/


        public Player(string name, bool isHuman)
        {
            Name = name;
            IsHuman = isHuman;
            Status = Status.Playing;
            Hand = new Hand();
        }

        /// <summary>
        /// Returns the next action the Player intends to execute.
        /// </summary>
        /// <returns></returns>        
        public abstract GameEvent GetAction(Game game);


        public override string ToString()
        {
            return Name + (IsHuman ? "*" : "");
        }
    }
}