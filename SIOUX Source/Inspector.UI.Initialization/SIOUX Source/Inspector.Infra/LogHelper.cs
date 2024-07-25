/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Diagnostics;
using log4net;

namespace Inspector.Infra
{
    public static class LogHelper
    {
        /// <summary>
        /// Gets or sets a value indicating whether [debug extended].
        /// only used in Visual Studio Enviroment for extended debugging 
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug extended]; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugExtended { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public enum LogLevel
        {
            Info,
            Debug,
            Warning,
            Error
        }

        /// <summary>
        /// Logs the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logline">The logline.</param>
        public static void Log(ILog logger, LogLevel logLevel, string logline)
        {
            if (logger != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                        Debug.WriteLineIf(DebugExtended, logline);
                        logger.Info(logline);
                        break;

                    case LogLevel.Debug:
                        Debug.WriteLineIf(DebugExtended, logline);
                        logger.Debug(logline);
                        break;

                    case LogLevel.Warning:
                        Debug.WriteLine(logline);
                        logger.Warn(logline);
                        break;

                    case LogLevel.Error:
                        Debug.WriteLine(logline);
                        logger.Error(logline);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
