using UnityEngine;

namespace com.szczuro.slots.game
{
    public interface IPayLineCollector
    {
        bool CheckPayLine(int[] payline);
    }

    public class BasePayline : ScriptableObject, IPayLineCollector
    {
        public bool CheckPayLine(int[] payline)
        {
            throw new System.NotImplementedException();
        }
    }

}
