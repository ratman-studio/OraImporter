
using UnityEngine;
namespace com.szczuro.slots.data
{
    [CreateAssetMenu(fileName = "InfoDat", menuName = "Slots SO/Slot Info", order = 1)]
    public class InfoData:ScriptableObject
    {
        public int WaysToWin;
        public int TotalWaysToWin;
        public int TotalPayout;
        public int TotalWays;
        public float HitFrequency;
        public float ReturnToPlayer;
        public float AverageSpinsUntilWin;
    }
}