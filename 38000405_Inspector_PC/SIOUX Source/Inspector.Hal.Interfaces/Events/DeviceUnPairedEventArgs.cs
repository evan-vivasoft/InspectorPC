using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspector.Hal.Interfaces.Events
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "UnPaired")]
    public class DeviceUnPairedEventArgs : EventArgs
    {
        public string Address { get; set; }

        public DeviceUnPairedEventArgs(string address)
        {
            Address = address;
        }
    }
}
