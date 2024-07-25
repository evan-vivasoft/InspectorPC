using System.Collections.ObjectModel;
using Inspector.BusinessLogic.Data.Configuration.HardwareConfiguration;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Configuration.Test
{
    /// <summary>
    /// BluetoothDongleInformationManagerTest
    /// </summary>
    [TestFixture]
    public class BluetoothDongleInformationManagerTest
    {
        [Test]
        public void RetrieveAvailableBluetoothDonglesTest()
        {
            BluetoothDongleInformationManager btDongleManager = new BluetoothDongleInformationManager();
            ReadOnlyCollection<string> btDongles = btDongleManager.RetrieveAvailableBluetoothDongles("baBlueSoleil");

            Assert.AreEqual(1, btDongles.Count);
            Assert.AreEqual("(00:00:00:00:00:00)", btDongles[0]);
        }

    }
}
