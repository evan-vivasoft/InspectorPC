/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.BluetoothDongle
{
    /// <summary>
    /// BluetoothDongleInformation
    /// </summary>
    public class BluetoothDongleInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothDongleInformation"/> class.
        /// </summary>
        /// <param name="bluetoothAddress">The bluetooth address.</param>
        public BluetoothDongleInformation(string bluetoothAddress)
        {
            BluetoothAddress = bluetoothAddress;
        }

        /// <summary>
        /// Gets or sets the bluetooth address.
        /// </summary>
        /// <value>
        /// The bluetooth address.
        /// </value>
        public string BluetoothAddress { get; private set; }
    }
}
