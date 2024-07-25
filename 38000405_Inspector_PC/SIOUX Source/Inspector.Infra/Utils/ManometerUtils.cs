/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;

namespace Inspector.Infra.Utils
{
    /// <summary>
    /// This class contains specifications on the manometers
    /// </summary>
    public static class ManometerUtils
    {
        private static Dictionary<string, int> s_ManometerResolution;

        /// <summary>
        /// Fills the manometer resolution.
        /// </summary>
        private static void FillManometerResolution()
        {
            if (s_ManometerResolution == null)
            {
                s_ManometerResolution = new Dictionary<string, int>
                {
                    {"25mbar", 3},
                    {"70mbar", 3},
                    {"200mbar", 2},
                    {"300mbar", 2},
                    {"500mbar", 2},
                    {"1000mbar", 2},
                    {"1100mbar", 2},
                    {"2000mbar", 1},
                    {"7500mbar", 1},
                    {"10bar", 4},
                    {"17bar", 4},
                    {"35bar", 3},
                    {"70bar", 3},
                    {"90bar", 3}
                };
            }
        }

        /// <summary>
        /// Gets the resolution for manometer pressure.
        /// </summary>
        /// <param name="pressureRange">The pressure range.</param>
        /// <returns>The resolution for the give pressure range</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given pressure range cannot be found.</exception>
        public static int GetResolutionForManometerPressure(string pressureRange)
        {
            int resolution = 0;
            FillManometerResolution();

            if (s_ManometerResolution.ContainsKey(pressureRange))
            {
                resolution = s_ManometerResolution[pressureRange];
            }
            else
            {
                throw new ArgumentOutOfRangeException(pressureRange, "The manometer pressure provided cannot be found.");
            }

            return resolution;
        }
    }
}
