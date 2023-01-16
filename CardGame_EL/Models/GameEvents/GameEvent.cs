namespace CardGame_EL.GameEvents
{
    /// <summary>
    /// Abstract base class representing events that take place during the game
    /// Class is used instead of an interface so that GameStateActions and PlayerActions
    /// are of the same type.
    /// </summary>
    public abstract class GameEvent
    {        
        public int Id { get; set; }
        public abstract void Execute();
    }
}
