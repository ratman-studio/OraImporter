
using com.szczuro.slots.data;

using NUnit.Framework;

namespace com.szczuro.slots.info
{
    class SlotInfoBase_Test
    {
        static public SlotInfoBase PrepareEmptySlotInfo()
        {
            var slotEmptyData = SlotDataTest.PrepareEmptySlotData();
            return new SlotInfoBase(slotEmptyData);
        }

        ISlotInfo infoEmpty;
        ISlotInfo info;
        [SetUp]
        public void BeforeTest()
        {
            infoEmpty = PrepareEmptySlotInfo();
        }

        [Test]
        public void EmptyCreated()
        {
            Assert.IsNotNull(infoEmpty);
        }

        [Test]
        public void EmptyBase_StatsAre0()
        {
            Assert.AreEqual(infoEmpty.TotalWaysToWin,1f);
            
            

        }
        [Test]
        public void InfoCreated()
        {
            Assert.IsNotNull(info);
        }

        [Test]
        public void WaysToWin_IsAbove0()
        {
            Assert.Greater(info.WaysToWin, 0);
        }
    }

}
