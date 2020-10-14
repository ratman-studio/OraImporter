namespace com.szczuro.slots.game
{
    /// <summary>
    /// All posible actions by the user 
    /// </summary>
    public interface ISlotUserActions
    {
        void InsertCoin(int amount = 1);
        int IncreaseBet(int amount = 1);
        int DecreaseBet(int amount = 1);
        int Spin();
        int PayCheck();
    }
}
