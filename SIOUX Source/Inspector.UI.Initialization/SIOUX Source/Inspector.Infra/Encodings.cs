/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Text;

namespace Inspector.Infra
{
    /// <summary>
    /// Custom file encodings that are not directly encoded into the .NET framework
    /// </summary>
    public static class Encodings
    {
        /// <summary>
        /// Gets the 'ANSI-encoding'.
        /// Note that 'ANSI' is not really an encoding but most often 
        /// this encoding is mistakenly referred to as the 'ANSI-encoding'.
        /// </summary>
        public static Encoding WindowsAnsi
        {
            get
            {
                return Encoding.GetEncoding(1252);
            }
        }

    }
}
