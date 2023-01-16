namespace CardGame_EL.GameEvents;

/// <summary>
/// The abstract class representing actions taken that change the game state
/// </summary>
public abstract class GameStateAction : GameEvent
{

}

/// <summary>
/// Signal the end of the game
/// </summary>
public class GameOverEvent : GameStateAction
{
    public override void Execute()
    {
        // do nothing
    }

    public override string ToString()
    {
        return "[" + nameof(GameOverEvent) + "]";
    }
}