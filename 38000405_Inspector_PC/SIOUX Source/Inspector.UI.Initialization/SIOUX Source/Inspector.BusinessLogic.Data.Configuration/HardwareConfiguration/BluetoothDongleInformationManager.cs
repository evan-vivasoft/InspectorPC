/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Infra.Ioc;

namespace Inspector.BusinessLogic.Data.Configuration.HardwareConfiguration
{
    /// <summary>
    /// BluetoothDongleInformationManager
    /// </summary>
    public class BluetoothDongleInformationManager : IBluetoothDongleInformationManager
    {
        private IHal m_Hal;

        public IHal Hal
        {
            get
            {
                if (m_Hal == null)
                {
                    m_Hal = ContextRegistry.Context.Resolve<IHal>();
                }
                return m_Hal;
            }
            set
            {
                m_Hal = value;
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothDongleInformationManager"/> class.
        /// </summary>
        public BluetoothDongleInformationManager()
        {
        }
        #endregion Constructors

        #region IBluetoothDongleInformationManager Members
        /// <summary>
        /// Retrieves the list of available BluetoothDongles addresses for the given Bluetooth API that are connected to the PC.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <returns>The list of Bluetooth dongle addresses, or an empty list if not Bluetooth dongles for the API are found</returns>
        /// <exception cref="HardwareConfigurationException">Thrown when the available bluetooth dongles could not be retrieved</exception>
        public ReadOnlyCollection<string> RetrieveAvailableBluetoothDongles(string bluetoothApi)
        {
            List<string> availableBluetoothDongles;

            try
            {
                availableBluetoothDongles = Hal.RetrieveAvailableBluetoothDongles(bluetoothApi);
            }
            catch (ConnectionException)
            {
                throw new HardwareConfigurationException("Could not retrieve available bluetooth dongles.");
            }

            return new ReadOnlyCollection<string>(availableBluetoothDongles);
        }
        #endregion IBluetoothDongleInformationManager Members
    }
}
