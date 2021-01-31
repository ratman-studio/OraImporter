
using com.szczuro.slots.data;

using NUnit.Framework;

namespace com.szczuro.slots.info
{
    class SlotInfoBase_Test
    {
        private static SlotInfoBase PrepareEmptySlotInfo()
        {
            var slotEmptyData = SlotTests.PrepareEmptySlotData();
            return new SlotInfoBase(slotEmptyData);
        }

        ISlotInfo _infoEmpty;
        ISlotInfo _info;
        [SetUp]
        public void BeforeTest()
        {
            _infoEmpty = PrepareEmptySlotInfo();
        }

        [Test]
        public void EmptyCreated()
        {
            Assert.IsNotNull(_infoEmpty);
        }

        [Test]
        public void EmptyBase_StatsAre0()
        {
            Assert.AreEqual(_infoEmpty.TotalWaysToWin,1f);
            
            

        }
        [Test]
        public void InfoCreated()
        {
            Assert.IsNotNull(_info);
        }

        [Test]
        public void WaysToWin_IsAbove0()
        {
            Assert.Greater(_info.WaysToWin, 0);
        }
    }

}
