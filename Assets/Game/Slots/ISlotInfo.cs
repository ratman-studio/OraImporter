namespace com.szczuro.slots.info
{
    public interface ISlotInfo
    {
        int WaysToWin { get; }
        float TotalWaysToWin { get; }
        int TotalPayout { get; }
        int TotalWays { get; }
        float HitFrequency { get; }
        float ReturnToPlayer { get; }
        float AverageSpinsUntilWin { get; }
    }
}