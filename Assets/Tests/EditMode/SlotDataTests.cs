using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


namespace com.szczuro.slots.data
{
    public class SlotTests
    {
        static public SlotData PrepareTestSlotData()
        {
            
            SlotData slot = ScriptableObject.CreateInstance<SlotData>();
            slot.MaxBet = 10;
            slot.MinBet = 5;
            slot.Name = "SlotName";
            slot.Paylines = 7;
            slot.StopTypes = new List<string>() { "one", "two", "three", "four" };

            var l1 = new List<int>() { 1, 2, 3, 4, 1 };
            var l2 = new List<int>() { 4, 3, 2, 1, 2 };
            var l3 = new List<int>() { 2, 1, 2, 1, 3 };

            var reel1 = new ReelWheel(l1);
            var reel2 = new ReelWheel(l2);
            var reel3 = new ReelWheel(l3);
            
            slot.Reels = new List<ReelWheel>() { reel1, reel2, reel3 };

            var p1 = new List<int>() { 1, 2, 3, 4, 1 };
            var p2 = new List<int>() { 1, 2, 3, 4, 1 };
            var p3 = new List<int>() { 1, 2, 3, 4, 1 };
            
            var po1 = new PayOut(1, p1);
            var po2 = new PayOut(2, p2);
            var po3 = new PayOut(3, p3);
            
            slot.PayTable = new List<PayOut>() { po1, po2, po3 };

            return slot;
        }

        static public SlotData PrepareEmptySlotData()
        {
            return ScriptableObject.CreateInstance<SlotData>();
        }
    }
    public class SlotDataTest : SlotTests
    {
        SlotData slot ;
        SlotData slotEmpty ;
        [SetUp]
        public void BeforeTest()
        {
            slot = PrepareTestSlotData();
            slotEmpty = PrepareEmptySlotData();
        }

        [Test]
        public void MaxBet_IsSet()
        {
            Assert.AreEqual(10, slot.MaxBet);
        }
        [Test]
        public void MinBet_IsSet()
        {
            Assert.AreEqual(5, slot.MinBet);
        }
        [Test]
        public void Name_IsSet()
        {
            Assert.AreEqual("SlotName", slot.Name);
        }
        [Test]
        public void Paylines_AreSet()
        {
            Assert.AreEqual(7, slot.Paylines);
        }
        [Test]
        public void Stoptypes_AreSet()
        {
            Assert.AreNotEqual(null, slot.StopTypes);
            Assert.AreEqual(4, slot.StopTypes.Count);
            Assert.AreEqual("one", slot.StopTypes[0]);
            Assert.AreEqual("two", slot.StopTypes[1]);
            Assert.AreEqual("three", slot.StopTypes[2]);
            Assert.AreEqual("four", slot.StopTypes[3]);
        }
        [Test]
        public void Reels_AreSet()
        {

            var l1 = new List<int>() { 1, 2, 3, 4, 1 };
            var l2 = new List<int>() { 4, 3, 2, 1, 2 };
            var l3 = new List<int>() { 2, 1, 2, 1, 3 };

            Assert.AreNotEqual(null, slot.Reels);
            Assert.AreEqual(3, slot.Reels.Count);
            Assert.AreEqual(l1, slot.Reels[0].colors);
            Assert.AreEqual(l2, slot.Reels[1].colors);
            Assert.AreEqual(l3, slot.Reels[2].colors);
   
        }

        [Test]
        public void PayTable_IsSet()
        {
            var p1 = new List<int>() { 1, 2, 3, 4, 1 };
            var p2 = new List<int>() { 1, 2, 3, 4, 1 };
            var p3 = new List<int>() { 1, 2, 3, 4, 1 };

            Assert.AreNotEqual(null, slot.Reels);
            Assert.AreEqual(3, slot.PayTable.Count);

            Assert.AreEqual(p1, slot.PayTable[0].colors);
            Assert.AreEqual(p2, slot.PayTable[1].colors);
            Assert.AreEqual(p3, slot.PayTable[2].colors);

            Assert.AreEqual(1, slot.PayTable[0].Multiplayer);
            Assert.AreEqual(2, slot.PayTable[1].Multiplayer);
            Assert.AreEqual(3, slot.PayTable[2].Multiplayer);
        }

        [Test]
        public void EmptyData()
        {
            // this data could be changed by designer
            Assert.AreEqual(1, slotEmpty.MaxBet);
            Assert.AreEqual(1, slotEmpty.MinBet);
            Assert.AreEqual(1, slotEmpty.Paylines);

            // this data is needed to be provided by designer
            Assert.AreEqual(null, slotEmpty.Name);
            Assert.AreEqual(0, slotEmpty.StopTypes.Count);
            Assert.AreEqual(null, slotEmpty.Reels, "Reels not found");
            Assert.AreEqual(null, slotEmpty.PayTable, "Payline not found");
        }
    }
  
}
