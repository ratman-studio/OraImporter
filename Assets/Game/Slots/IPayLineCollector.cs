using com.szczuro.slots.data;

using System.Collections.Generic;

using UnityEngine;

namespace com.szczuro.slots.game
{
    public interface IPayLineCollector
    {
        
        bool CheckPayLine(int[] payline);
    }

    public class BasePayline : ScriptableObject, IPayLineCollector
    {
        public SlotData slotData;

        public bool CheckPayLine(int[] payline)
        {          
            throw new System.NotImplementedException();
        }
    }

}
