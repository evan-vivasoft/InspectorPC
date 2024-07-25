/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Windows.Data;
using Inspector.Model;


namespace Inspector.UI.Initialization
{
    public class InitializationStepResultConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageSource;
            if (value is InitializationStepResult)
            {
                InitializationStepResult result = (InitializationStepResult)value;
                switch (result)
                {
                    case InitializationStepResult.UNSET:
                        imageSource = null;
                        break;
                    case InitializationStepResult.SUCCESS:
                        imageSource = @"pack://application:,,,/Inspector.UI.Initialization;component/Images/success.png";
                        break;
                    case InitializationStepResult.ERROR:
                        imageSource = @"pack://application:,,,/Inspector.UI.Initialization;component/Images/error.png";
                        break;
                    case InitializationStepResult.WARNING:
                        imageSource = @"pack://application:,,,/Inspector.UI.Initialization;component/Images/warning.png";
                        break;
                    case InitializationStepResult.TIMEOUT:
                        imageSource = @"pack://application:,,,/Inspector.UI.Initialization;component/Images/timeout.png";
                        break;
                    default:
                        // Cannot occur
                        throw new ArgumentException("Could not convert the InitializationStepResult input value");
                }
            }
            else
            {
                throw new ArgumentException("Can only convert InitializationStepResult");
            }

            return imageSource;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
