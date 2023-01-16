using CardGame_EL.Enums;

namespace CardGame_EL.Models
{
    /// <summary>
    /// Represents a playing card
    /// </summary>
    public class Card
    {
        public int Id { get; set; }
        public Suit Suit { get; set; }
        public Value Value { get; set; }


        public Card(Suit suit, Value value)
        {
            Suit = suit;
            Value = value;
        }

        public Card()
        {
        }

        public override string ToString()
        {
            return Value + " of " + Suit;
        }
    }
}