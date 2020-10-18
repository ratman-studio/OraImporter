
using com.szczuro.slots.data;

using System;

using UnityEngine;
namespace com.szczuro.slots.info
{
    public class SlotInfoBase:ISlotInfo
    {
private SlotData slotData;

        public SlotInfoBase(SlotData slotData)
        {
            this.slotData = slotData;
        }

        public int WaysToWin => throw new NotImplementedException();

        public float TotalWaysToWin => throw new NotImplementedException();

        public int TotalPayout => throw new NotImplementedException();

        public int TotalWays => throw new NotImplementedException();

        public float HitFrequency => throw new NotImplementedException();

        public float ReturnToPlayer => throw new NotImplementedException();

        public float AverageSpinsUntilWin => throw new NotImplementedException();
    }
}