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
                s_ManometerResolution = new Dictionary<string, int>();
                s_ManometerResolution.Add("25mbar", 3);
                s_ManometerResolution.Add("70mbar", 3);
                s_ManometerResolution.Add("200mbar", 2);
                s_ManometerResolution.Add("300mbar", 2);
                s_ManometerResolution.Add("500mbar", 2);
                s_ManometerResolution.Add("1000mbar", 2);
                s_ManometerResolution.Add("1100mbar", 2);
                s_ManometerResolution.Add("2000mbar", 1);
                s_ManometerResolution.Add("7500mbar", 1);
                s_ManometerResolution.Add("10bar", 4);
                s_ManometerResolution.Add("17bar", 4);
                s_ManometerResolution.Add("35bar", 3);
                s_ManometerResolution.Add("70bar", 3);
                s_ManometerResolution.Add("90bar", 3);
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
