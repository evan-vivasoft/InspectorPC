/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.ObjectModel;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// IBluetoothDongleInformationManager interface
    /// </summary>
    public interface IBluetoothDongleInformationManager
    {
        /// <summary>
        /// Retrieves the list of available BluetoothDongles addresses for the given Bluetooth API that are connected to the PC.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <returns>The list of Bluetooth dongle addresses, or an empty list if not Bluetooth dongles for the API are found</returns>
        /// <exception cref="HardwareConfigurationException">Thrown when the available bluetooth dongles could not be retrieved</exception>
        ReadOnlyCollection<string> RetrieveAvailableBluetoothDongles(string bluetoothApi);
    }
}
