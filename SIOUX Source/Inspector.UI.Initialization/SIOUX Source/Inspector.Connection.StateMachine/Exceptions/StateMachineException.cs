/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.Connection.SM.Exceptions
{
    /// <summary>
    /// Occurs when an invalid transaction takes place within the statemachine
    /// </summary>
    [Serializable]
    public class StateMachineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class.
        /// </summary>
        public StateMachineException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public StateMachineException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public StateMachineException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected StateMachineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
