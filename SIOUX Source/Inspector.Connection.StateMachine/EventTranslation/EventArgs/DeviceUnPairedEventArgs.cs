using System;

namespace Inspector.Connection
{
    public class DeviceUnPairedEventArgs : EventArgs
    {
        public string Address { get; set; }

        public DeviceUnPairedEventArgs(string address)
        {
            Address = address;
        }
    }
}
