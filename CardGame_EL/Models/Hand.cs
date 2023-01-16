using Utilities;

namespace CardGame_EL.Models
{
    public class Hand : Deck
    {
        /// <summary>
        /// Use this constructor from base-class to ensure an empty list of cards
        /// </summary>
        public Hand() : base(new ListManager<Card>())
        {

        }



        // override methods from base if desired
    }
}