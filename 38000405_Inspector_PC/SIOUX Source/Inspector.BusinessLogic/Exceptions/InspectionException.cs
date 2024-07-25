/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.BusinessLogic.Exceptions
{
    /// <summary>
    /// Indicates that an exception occured during an inspection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    [Serializable]
    internal class InspectionException : Exception
    {
        /// <summary>
        /// Gets or sets the inspection procedure result.
        /// </summary>
        /// <value>The inspection procedure result.</value>
        public InspectionProcedureResult InspectionProcedureResult { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        /// <param name="inspectionProcedureResult">The inspection procedure result.</param>
        /// <param name="errorCode">The error code.</param>
        public InspectionException(InspectionProcedureResult inspectionProcedureResult, int errorCode) : this(null, inspectionProcedureResult, errorCode) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inspectionProcedureResult">The inspection procedure result.</param>
        /// <param name="errorCode">The error code.</param>
        public InspectionException(string message, InspectionProcedureResult inspectionProcedureResult, int errorCode)
            : base(message)
        {
            InspectionProcedureResult = inspectionProcedureResult;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        public InspectionException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InspectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InspectionException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InspectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
