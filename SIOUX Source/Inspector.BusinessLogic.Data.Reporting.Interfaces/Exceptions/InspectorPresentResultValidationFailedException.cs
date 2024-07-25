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
    /// InspectorPresentResultValidationFailedException
    /// </summary>
    [Serializable]
    public class InspectorPresentResultValidationFailedException : InspectorReportControlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPresentResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InspectorPresentResultValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPresentResultValidationFailedException"/> class.
        /// </summary>
        public InspectorPresentResultValidationFailedException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPresentResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InspectorPresentResultValidationFailedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPresentResultValidationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public InspectorPresentResultValidationFailedException(string message, Exception exception) : base(message, exception) { }
    }
}

