/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Runtime.Serialization;

namespace Inspector.BusinessLogic.Data.Reporting.Interfaces
{
    /// <summary>
    /// InspectorResultValidationFailedException
    /// </summary>
    [Serializable]
    public class InspectorResultValidationFailedException : InspectorReportControlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InspectorResultValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorResultValidationFailedException"/> class.
        /// </summary>
        public InspectorResultValidationFailedException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InspectorResultValidationFailedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public InspectorResultValidationFailedException(string message, Exception exception) : base(message, exception) { }
    }
}

