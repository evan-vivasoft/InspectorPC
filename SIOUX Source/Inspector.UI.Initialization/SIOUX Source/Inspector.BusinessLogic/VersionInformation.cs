/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// VersionInformation
    /// </summary>
    public static class VersionInformation
    {
        /// <summary>
        /// Gets the component version.
        /// </summary>
        public static string ComponentVersion
        {
            get
            {
                Version versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string version = versionInfo.Major + "." + versionInfo.Minor + "." + versionInfo.Build + "." + versionInfo.Revision;
                return version;
            }
        }

        /// <summary>
        /// Gets the assembly information.
        /// </summary>
        public static System.Reflection.Assembly AssemblyInformation
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly();
            }
        }
    }
}
