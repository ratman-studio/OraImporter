
using System;

using UnityEngine;
namespace com.szczuro.slots.data
{

    [CreateAssetMenu(fileName = "InfoDat", menuName = "Slots SO/Slot Info", order = 1)]
    public class SlotInfoData:ScriptableObject
    {
        public int WaysToWin;
        public int TotalWaysToWin;
        public int TotalPayout;
        public int TotalWays;
        public float HitFrequency;
        public float ReturnToPlayer;
        public float AverageSpinsUntilWin;
        private SlotData slotData;

        public SlotInfoData(SlotData slotData)
        {
            this.slotData = slotData;
        }

        public bool Validate()
        {
            return false;
        }
    }
}