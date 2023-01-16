using CardGame_EL.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace CardGame_EL.Models
{
    /// <summary>
    /// Represents a deck of playing cards
    /// </summary>
    public class Deck
    {
        public int Id { get; set; }
        public ICollection<Card> Cards
        {
            get => CardsManager.List;
            set => CardsManager.List = (List<Card>)value;
        }

        [NotMapped]
        private const int NR_SUITS = 4;
        [NotMapped]
        private const int NR_CARDS_PER_SUIT = 13;
        [NotMapped]
        public int CardCount => CardsManager.Count;
        [NotMapped]
        public int Score => CardsManager.List.Sum(card => (int)card.Value);
        [NotMapped]
        private ListManager<Card> CardsManager { get; set; }

        /**********************************************************************
        * Constructors
        /**********************************************************************/

        /// <summary>
        /// Constructor that initializes the deck of cards as a traditional 52 Ace-King deck.
        /// </summary>
        public Deck()
        {
            CardsManager = new ListManager<Card>();
            //     LoadNewDeck();
        }
        /// <summary>
        /// A constructor allowing the caller to set the deck of cards themselves.
        /// </summary>
        /// <param name="nrSuits"></param>
        /// <param name="nrCardsInSuit"></param>
        /// <param name="cards"></param>
        public Deck(List<Card> cards)
        {
            // NrSuits = nrSuits;
            //  NrCardsInSuit = nrCardsInSuit;
            CardsManager = new ListManager<Card>(cards);
        }

        /// <summary>
        /// Use this constructor for derived classes wanting to start with an empty deck of cards.
        /// </summary>
        /// <param name="cardsManager"></param>
        protected Deck(ListManager<Card> cardsManager)
        {
            CardsManager = cardsManager;
        }

        /**********************************************************************
         * Public methods
        /**********************************************************************/

        public virtual void Shuffle()
        {
            CardsManager.Shuffle();
        }

        public virtual Card DrawCard()
        {
            return CardsManager.Pop();
        }

        public virtual void AddCard(Card card)
        {
            CardsManager.Add(card);
        }

        public virtual void AddCard(Suit suit, Value value)
        {
            AddCard(new Card(suit, value));
        }

        public void DiscardCard(Suit suit, Value value)
        {
            var card = Cards
                .Where(c => c.Value.Equals(value) && c.Suit.Equals(suit))
                .First();

            this.DiscardCard(card);
        }

        public virtual void DiscardCard(Card card)
        {
            CardsManager.Delete(card);
        }

        public void LoadNewDeck(int numberDecks)
        {
            CardsManager = new ListManager<Card>(CreateDeck(numberDecks));
        }

        /**********************************************************************
         * Private methods
        /**********************************************************************/
        /// <summary>
        /// Add all the cards necessary to create the deck.
        /// This is based on the Properties defining the number of suits and number of values in each suit.
        /// </summary>
        /// <returns></returns>
        private List<Card> CreateDeck(int numberDecks)
        {
            var loadCards = new List<Card>();

            for (int suit = 1; suit <= NR_SUITS; suit++)
            {
                for (int value = 1; value <= NR_CARDS_PER_SUIT; value++)
                {
                    loadCards.Add(new Card((Suit)suit, (Value)value));
                }
            }

            // more than 1 deck of cards is required
            if (numberDecks > 1)
            {
                for (int i = 0; i < numberDecks; i++)
                {
                    loadCards.AddRange(loadCards.ToList());
                }
            }
            return loadCards;
        }
    }
}