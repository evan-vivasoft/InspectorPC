/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.Hal.Interfaces.Exceptions
{
    /// <summary>
    /// Used for checksum related errors
    /// </summary>
    [Serializable]
    public class ChecksumException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumException"/> class.
        /// </summary>
        public ChecksumException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ChecksumException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ChecksumException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ChecksumException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
