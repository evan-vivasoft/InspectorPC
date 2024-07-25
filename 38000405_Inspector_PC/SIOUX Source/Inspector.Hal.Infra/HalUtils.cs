/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.Hal.Interfaces;
using Inspector.Infra.Ioc;

namespace Inspector.Hal.Infra
{
    /// <summary>
    /// HalUtils class
    /// </summary>
    public class HalUtils
    {
        /// <summary>
        /// Determines whether [is demo mode].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is demo mode]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDemoMode()
        {
            IHal hal = ContextRegistry.Context.Resolve<IHal>();
            bool isDemoMode = (hal is BluetoothHalDemoMode);
            return isDemoMode;
        }
    }
}
