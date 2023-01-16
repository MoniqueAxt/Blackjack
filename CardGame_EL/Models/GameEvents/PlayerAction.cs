using CardGame_EL.Enums;
using CardGame_EL.Models;
using CardGame_EL.Players;

namespace CardGame_EL.GameEvents;

/// <summary>
/// The abstract class representing all Player-driven actions
/// </summary>
public abstract class PlayerAction : GameEvent
{
    public Player Owner { get; set; } = null!;

    protected PlayerAction(Player owner)
    {
        Owner = owner;
    }

    protected PlayerAction() { }
}

/// <summary>
/// Signal that user input/gameEvent is required
/// </summary>
public class UserInputRequired : PlayerAction
{
    /// <summary>
    /// EF constructor
    /// </summary>
    private UserInputRequired() { }

    public UserInputRequired(Player owner) : base(owner)
    { }

    public override string ToString()
    {
        return Owner + "[" + nameof(UserInputRequired) + "]";
    }

    public override void Execute()
    {
        // do nothing
    }
}

/// <summary>
/// Signal that no action is taken
/// </summary>
public class NoAction : PlayerAction
{
    /// <summary>
    /// EF constructor
    /// </summary>
    private NoAction() { }

    public NoAction(Player owner) : base(owner)
    { }

    public override void Execute()
    {
        // do nothing
    }

    public override string ToString()
    {
        return Owner + " [" + nameof(NoAction) + "]";
    }
}

/// <summary>
/// Take a card from the top of the Card Deck and place it into the relevant hand
/// </summary>
public class ReceiveCardEvent : PlayerAction
{
    public Deck ToHand { get; private set; }
    public Deck FromDeck { get; private set; }
    public Card DealtCard { get; private set; }

    public override void Execute()
    {
        DealtCard = FromDeck.DrawCard();
        ToHand.AddCard(DealtCard);
        Owner.Status = Status.Playing;
    }

    /// <summary>
    /// EF constructor
    /// </summary>
    private ReceiveCardEvent() { }

    public ReceiveCardEvent(Deck fromDeck, Hand toHand, Player owner) : base(owner)
    {
        FromDeck = fromDeck;
        ToHand = toHand;
        DealtCard = default!;
    }

    public override string ToString()
    {
        return Owner + " [" + nameof(ReceiveCardEvent) + "] " + DealtCard ?? "";
    }

}

/// <summary>
/// Take a card from the top of the Card Deck and place it into the relevant hand
/// </summary>
public class DrawCardEvent : PlayerAction
{
    public Deck ToHand { get; private set; }
    public Deck FromDeck { get; private set; }
    public Card DrawnCard { get; private set; }

    public override void Execute()
    {
        DrawnCard = FromDeck.DrawCard();
        ToHand.AddCard(DrawnCard);
    }

    /// <summary>
    /// EF constructor
    /// </summary>
    private DrawCardEvent() { }

    public DrawCardEvent(Deck fromDeck, Hand toHand, Player owner) : base(owner)
    {
        FromDeck = fromDeck;
        ToHand = toHand;
        DrawnCard = default!;
    }

    public override string ToString()
    {
        return Owner + " [" + nameof(DrawCardEvent) + "] " + DrawnCard ?? "";
    }

}

/// <summary>
/// Remove a specific card from this deck
/// </summary>
public class DiscardCardEvent : PlayerAction
{
    public Deck Hand { get; private set; }
    public Card DiscardedCard { get; private set; }

    public override void Execute()
    {
        Hand.DiscardCard(DiscardedCard);
    }

    /// <summary>
    /// EF constructor
    /// </summary>
    private DiscardCardEvent() { }

    public DiscardCardEvent(Deck hand, Card discardedCard, Player owner) : base(owner)
    {
        Hand = hand;
        DiscardedCard = discardedCard;
    }

    public override string ToString()
    {
        return Owner + " [" + nameof(DiscardCardEvent) + "] " + DiscardedCard;
    }
}