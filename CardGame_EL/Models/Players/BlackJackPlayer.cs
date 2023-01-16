using CardGame_EL.Enums;
using CardGame_EL.GameEvents;
using CardGame_EL.Models;

namespace CardGame_EL.Players;

/// <summary>
/// Represents a Blackjack player
/// </summary>
public class BlackJackPlayer : Player
{
    public int Money { get; set; }

    public BlackJackPlayer(string name, bool isHuman) : base(name, isHuman)
    {
        Money = 100;
    }

    public override GameEvent GetAction(Game game)
    {
        // Draw card when the player is the dealer
        if (game.Dealer == this)
        {
            if (Score < 17)
            {
                bool allPlayersBust = true;

                foreach (Player player in game.Players)
                {
                    if (player == this)
                        continue;
                    if (!player.HasLost)
                        allPlayersBust = false;
                }

                if (!allPlayersBust)
                {
                    return DrawCard(game.GameDeck);
                }
            }
        }

        // Draw card when the player is NOT the dealer
        else
        {
            bool dealerHasGoodCard = game.Dealer.Hand.Cards.ElementAt(0).Value >= Value.Seven;
            bool dealerHasFairCard = game.Dealer.Hand.Cards.ElementAt(0).Value is Value.Two or Value.Three;
            bool dealerHasPoorCard = game.Dealer.Hand.Cards.ElementAt(0).Value is Value.Four or Value.Five or Value.Six;

            // hit
            if (dealerHasGoodCard && Score < 17 ||
                dealerHasFairCard && Score < 13 ||
                dealerHasPoorCard && Score < 12)
            {
                return DrawCard(game.GameDeck);
            }
        }

        // stand
        return Stand();
    }

    private GameEvent DrawCard(Deck deck)
    {
        Status = Status.Playing;
        return new DrawCardEvent(deck, Hand, this);
    }

    private GameEvent Stand()
    {
        Status = Status.Standing;
        return new NoAction(this);
    }
}