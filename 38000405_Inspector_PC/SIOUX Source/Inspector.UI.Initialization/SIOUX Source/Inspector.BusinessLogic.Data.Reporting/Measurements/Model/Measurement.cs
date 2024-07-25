/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// 
    /// </summary>
    internal class Measurement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Data Data { get; set; }

        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>
        /// The extra data.
        /// </value>
        public ExtraData ExtraData { get; set; }

        /// <summary>
        /// Gets or sets the data header.
        /// </summary>
        /// <value>
        /// The data header.
        /// </value>
        public DataHeader DataHeader { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (Data != null)
            {
                result.Append(Data.ToString());
            }

            if (ExtraData != null)
            {
                result.AppendLine(ExtraData.ToString());
            }
            else
            {
                result.AppendLine();
            }

            if (DataHeader != null)
            {
                result.AppendLine(DataHeader.ToString());
            }

            return result.ToString();
        }
    }
}
