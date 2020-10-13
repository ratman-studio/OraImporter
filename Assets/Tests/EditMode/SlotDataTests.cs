using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

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
            //slot.Reels = new List<List<int>> { reel1, reel2, reel3 };

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
        public void SlotData_IsSet()
        {
            SlotData slot = PrepareTestSlot();
            Assert.AreEqual(10, slot.MaxBet);
            Assert.AreEqual(5, slot.MinBet);
            Assert.AreEqual("SlotName", slot.Name);
            Assert.AreEqual(7, slot.Paylines);

            Assert.AreEqual(4, slot.StopTypes.Count);
            Assert.AreEqual("one", slot.StopTypes[0]);
        }

        [Test]
        public void SlotData_Empty()
        {
            SlotData slot = PrepareEmptySlot();

            // this data could be changed by designer
            Assert.AreEqual(1, slot.MaxBet);
            Assert.AreEqual(1, slot.MinBet);
            Assert.AreEqual(1, slot.Paylines);

            // this data is needed to be provided by designer
            Assert.AreEqual(null, slot.Name);
            Assert.AreEqual(0, slot.StopTypes.Count);
            //Assert.AreEqual(0, slot.Reels.Count);
        }
    }
    public class SlotData_Valid_Configuration: SlotTests
    {
        private SlotData[] sds;
        [SetUp]
        public void BeforeTest()
        {
            sds = TestHelper.GetAllInstances<SlotData>();
        }

        [Test]
        public void MinBet_LessThan_MaxBet()
        {
            foreach (SlotData slot in sds)
                Assert.LessOrEqual(slot.MinBet, slot.MaxBet);
        }
        [Test]
        public void MinBet_BiggerThan0()
        {
            foreach (SlotData slot in sds)
                Assert.GreaterOrEqual(slot.MinBet, 1);
        }

        [Test]
        public void StopType_TwoOrMore()
        {
            foreach (SlotData slot in sds)
                Assert.GreaterOrEqual(slot.StopTypes.Count, 2);
        }

        [Test]
        public void PayLines_OneOrMore()
        {
            foreach (SlotData slot in sds)
                Assert.GreaterOrEqual(slot.StopTypes.Count, 1);
        }
        [Test]
        public void Reels_Exists_And_Above0()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Reels);
                Assert.GreaterOrEqual(slot.Reels.Count(), 1);
            }
        }

        private bool SlotHasColor(SlotData slot, int color)
        {
            foreach (ReelWheel reel in slot.Reels)
                if (reel.colors.Contains(color))
                    return true;
            return false;
        }

        [Test]
        public void Reels_Have_All_StopTypes()
        {
            foreach (SlotData slot in sds)
                foreach (string color in slot.StopTypes)
                    Assert.IsTrue(SlotHasColor(slot, slot.StopTypes.IndexOf(color)));
        }

        [Test]
        public void Reels_Have_Only_Colors_From_StopTypes()
        {
            foreach (SlotData slot in sds)
                foreach (ReelWheel reel in slot.Reels)
                    foreach (int color in reel.colors)
                    {
                        Assert.GreaterOrEqual(color,0);
                        Assert.Less(color, slot.StopTypes.Count());
                    }
        }
        [Test]
        public void Reels_StopCount_MoreThan1()
        {
            foreach (SlotData slot in sds)
            {
                if (slot.Reels?.Count() == 0) Assert.Fail();

                foreach (ReelWheel reel in slot.Reels)
                {
                    Assert.IsNotNull(reel);
                    Assert.Greater(reel.colors.Count(), 1);
                }
            }
        }

    }

    class TestHelper
    {
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return a;
        }
    }
}
