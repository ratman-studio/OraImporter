
using System.Collections.Generic;

using UnityEngine;
namespace com.szczuro.slots.data
{
    [CreateAssetMenu(fileName = "SlotData", menuName = "Slots/SO/Slots/SlotData", order = 1)]
    public class SlotData : ScriptableObject
    {
        public string Name;
        public int MinBet;
        public int MaxBet;
        public int Paylines;

        public Dictionary<int, string> StopTypes;
        public List<Dictionary<int, int>> Reels;

    }
}