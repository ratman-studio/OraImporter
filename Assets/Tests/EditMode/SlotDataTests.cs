using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;


namespace com.szczuro.slots.data.tests
{
    public class SlotTests
    {
        static public SlotData PrepareTestSlot()
        {
            SlotData slot = ScriptableObject.CreateInstance<SlotData>();
            slot.MaxBet = 10;
            slot.MinBet = 5;
            slot.Name = "SlotName";
            slot.Paylines = 7;
            slot.StopTypes = new List<string>() { "one", "two", "three", "four" };

            List<int> reel1 = new List<int>() { 1, 2, 3, 4, 1 };
            List<int> reel2 = new List<int>() { 4, 3, 2, 1, 2 };
            List<int> reel3 = new List<int>() { 2, 1, 2, 1, 3 };
            slot.Reels = new List<List<int>> { reel1, reel2, reel3 };

            return slot;
        }

        static public SlotData PrepareEmptySlot()
        {
            return ScriptableObject.CreateInstance<SlotData>();
        }
    }
    public class SlotDataTest : SlotTests
    {
        [Test]
        public void SlotDataIsSet()
        {
            SlotData slot = PrepareTestSlot();
            Assert.AreEqual(10, slot.MaxBet);
            Assert.AreEqual(5, slot.MinBet);
            Assert.AreEqual("SlotName", slot.Name);
            Assert.AreEqual(7, slot.Paylines);

            Assert.AreEqual(4, slot.StopTypes.Count);
            Assert.AreEqual("one", slot.StopTypes[0]);

            Assert.AreEqual(3, slot.Reels.Count);
            Assert.AreEqual(5, slot.Reels[0].Count);
            Assert.AreEqual(1, slot.Reels[0][0]);
        }

        [Test]
        public void SlotDataEmpty()
        {
            SlotData slot = PrepareEmptySlot();

            // this data could be changed by designer
            Assert.AreEqual(1, slot.MaxBet);
            Assert.AreEqual(1, slot.MinBet);
            Assert.AreEqual(1, slot.Paylines);

            // this data is needed to be provided by designer
            Assert.AreEqual(null, slot.Name);
            Assert.AreEqual(0, slot.StopTypes.Count);
            Assert.AreEqual(0, slot.Reels.Count);
        }
    }
    public class SlotInfoTest : SlotTests
    {
        [Test]
        public void MinLessThanMax_Validates()
        {
            SlotData slotData = PrepareTestSlot();
            InfoData slotInfo = new InfoData(slotData);
            Assert.IsTrue(slotInfo.Validate());
        }

        public void MaxLessThanMin_Invalidate()
        {
            SlotData slotData = PrepareTestSlot();
            slotData.MaxBet = 1;
            InfoData slotInfo = new InfoData(slotData);
            Assert.IsTrue(slotInfo.Validate());
        }
    }
}
