
using System;
using System.Collections.Generic;

using UnityEngine;
namespace com.szczuro.slots.data
{
    [CreateAssetMenu(fileName = "SlotData", menuName = "Slots/SlotData", order = 1)]
    public class SlotData : ScriptableObject
    {
        public string Name;
        // this data could be changed by designer

        public int MinBet = 1;
        public int MaxBet = 1;
        public int Paylines = 1;

        // this data is needed to be provided by designer
        public List<string> StopTypes = new List<string>();
        public List<ReelWheel> Reels;
        public List<PayOut> Payouts;
    }
    
    [Serializable]
    public struct ReelWheel
    {
        public ReelWheel(List<int> colors)
        {
            this.colors = colors;
        }

        public List<int> colors;
    }
    [Serializable]
    public struct PayOut
    {
        public PayOut(int payout, List<int> colors)
        {
            this.colors = colors;
            this.payout = payout;
        }

        public int payout;
        public List<int> colors;

    }
}
