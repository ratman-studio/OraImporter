using UnityEngine;
using System.Collections;

namespace com.szczuro.slots.game
{
    public class BaseSlotMachine : MonoBehaviour, ISlotUserActions
    {
        public IPayLineCollector PayLine;

        public int DecreaseBet(int amount = 1)
        {
            throw new System.NotImplementedException();
        }

        public int IncreaseBet(int amount = 1)
        {
            throw new System.NotImplementedException();
        }

        public void InsertCoin(int amount = 1)
        {
            throw new System.NotImplementedException();
        }

        public int PayCheck()
        {
            throw new System.NotImplementedException();
        }

        public int Spin()
        {
            throw new System.NotImplementedException();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}