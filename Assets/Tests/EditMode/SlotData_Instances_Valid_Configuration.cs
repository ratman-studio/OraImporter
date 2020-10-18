using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace com.szczuro.slots.data.tests
{
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
    public class SlotData_Instances_Valid_Configuration: SlotTests
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

        private bool SlotReelsHasColor(SlotData slot, int color)
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
                    Assert.IsTrue(SlotReelsHasColor(slot, slot.StopTypes.IndexOf(color)));
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

        [Test]
        public void Payouts_Exists_And_Above0()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Payouts);
                Assert.GreaterOrEqual(slot.Payouts.Count(), 1);
            }
        }

        [Test]
        public void Payout_Count_Less_Than_Reels()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Payouts);
                foreach (PayOut p in slot.Payouts)
                    Assert.LessOrEqual(p.colors.Count, slot.Reels.Count);
            }
        }

        [Test]
        public void Payout_Colors_Count_More_Than1()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Payouts);
                foreach (PayOut p in slot.Payouts)
                    Assert.Greater(p.colors.Count, 1);
            }
        }

        [Test]
        public void Payout_Multiplayer_Above1()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Payouts);
                foreach (PayOut p in slot.Payouts)
                    Assert.GreaterOrEqual(p.Multiplayer, 1);
            }
        }
        [Test]
        public void Payout_StopType_Match_Reel()
        {
            foreach (SlotData slot in sds)
            {
                Assert.IsNotNull(slot.Payouts);
                foreach (PayOut p in slot.Payouts)
                    for (int i=0; i<p.colors.Count;i++)
                    Assert.IsTrue(slot.Reels[i].colors.Contains(p.colors[i]));
            }
        }

        [Test]
        public void Payouts_Have_All_StopTypes()
        {
            foreach (SlotData slot in sds)
                foreach (string color in slot.StopTypes)
                    Assert.IsTrue(SlotPayoutHasColor(slot, slot.StopTypes.IndexOf(color)));
        }
        private bool SlotPayoutHasColor(SlotData slot, int color)
        {
            foreach (PayOut peyout in slot.Payouts)
                if (peyout.colors.Contains(color))
                    return true;
            return false;
        }

        [Test]
        public void Payouts_Have_Only_Colors_From_StopTypes()
        {
            foreach (SlotData slot in sds)
                foreach (PayOut payout in slot.Payouts)
                    foreach (int color in payout.colors)
                    {
                        Assert.GreaterOrEqual(color, 0);
                        Assert.Less(color, slot.StopTypes.Count());
                    }
        }
    }
}
