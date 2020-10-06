
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
    }

}