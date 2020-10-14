using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace com.szczuro.slots.tests
{
    class MockSlotMachnie : game.ISlotUserActions
    {
        public int Currentbet = 0 ;
        public bool AlwaysWin = false;
        public const int MaxBet = 1;
        public const int MinBet = 1;

        public void InsertCoin(int amount = 1 )
        {

        }

        public int IncreaseBet(int amount = 1 )
        {
            Currentbet += amount;
            return Currentbet; 
        }
        public int DecreaseBet(int amount = 1)
        {
            throw new System.NotImplementedException();
        }
        public int Spin()
        {
            throw new System.NotImplementedException();
        }
        public int PayCheck()
        {
            throw new System.NotImplementedException();
        }

    }

}
